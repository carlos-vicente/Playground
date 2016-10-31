using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Core.Logging;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.Events
{
    public class EventStore : IEventStore
    {
        private readonly IEventSerializer _serializer;
        private readonly IEventRepository _repository;
        private readonly ILogger _logger;
        private readonly Func<Guid> _batchIdProviderFunc;

        public EventStore(
            IEventSerializer serializer,
            IEventRepository repository,
            ILogger logger,
            Func<Guid> batchIdProviderFunc)
        {
            _serializer = serializer;
            _repository = repository;
            _logger = logger;
            _batchIdProviderFunc = batchIdProviderFunc;
        }

        public async Task CreateEventStream<TAggregateRoot>(Guid streamId)
        {
            var type = typeof(TAggregateRoot);
            _logger.Debug($"Creating event stream with identifier {streamId} for type {type.FullName}");

            var doesStreamAlreadyExists = await _repository
                .CheckStream(streamId)
                .ConfigureAwait(false);

            if(doesStreamAlreadyExists)
                throw new InvalidOperationException($"Stream with id {streamId} already exists!");

            _logger.Debug($"Creating event stream with identifier {streamId} has it does not exist");
            await _repository
                .CreateStream(streamId, type.AssemblyQualifiedName)
                .ConfigureAwait(false);

            _logger.Debug($"Done creating event stream with identifier {streamId}");
        }

        public async Task StoreEvents(
            Guid streamId,
            long currentVersion,
            ICollection<DomainEvent> eventsToStore)
        {
            _logger
                .Debug($"Storing {eventsToStore.Count} events on stream {streamId} with current version {currentVersion}");

            var lastStoredEvent = await _repository
                .GetLast(streamId)
                .ConfigureAwait(false);

            if (lastStoredEvent != null && currentVersion < lastStoredEvent.EventId)
            {
                throw new InvalidOperationException($"Cant add new events on version {currentVersion} as current storage version is {lastStoredEvent.EventId}");
            }

            var batchId = _batchIdProviderFunc();

            var lastStoredEventId = 0L;
            if (lastStoredEvent != null)
                lastStoredEventId = lastStoredEvent.EventId;

            _logger
                .Debug($"Current stored version for stream {streamId} is {lastStoredEventId}");

            var events = eventsToStore
                .Select(e => new StoredEvent(
                    e.GetType().AssemblyQualifiedName,
                    e.Metadata.OccorredOn,
                    _serializer.Serialize(e as object),
                    batchId,
                    ++lastStoredEventId))
                .ToList();

            _logger
                .Debug("Sending events to repository");

            await _repository
                .Add(streamId, events)
                .ConfigureAwait(false);

            _logger
                .Debug("All events stored");
        }

        public async Task<ICollection<DomainEvent>> LoadSelectedEvents(
            Guid streamId,
            long fromEventId)
        {
            _logger.Debug($"Loading selected event stream for {streamId} after version {fromEventId}");

            var doesStreamExist = await _repository
                .CheckStream(streamId)
                .ConfigureAwait(false);

            if (!doesStreamExist)
                return null;

            _logger.Debug($"Get selected stored events for stream {streamId}");

            var storedEvents = await _repository
                .GetSelected(streamId, fromEventId)
                .ConfigureAwait(false);

            _logger.Debug("Convert selected stored events to domain events");

            var domainEvents = storedEvents?
                .Select(GetDomainEvent)
                .ToList() ?? new List<DomainEvent>();

            _logger.Debug($"Returning selected domain events for stream {streamId}");

            return domainEvents;
        }

        public Task<ICollection<DomainEvent>> LoadSelectedEvents(
            Guid streamId,
            long fromEventId,
            long toEventId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<DomainEvent>> LoadAllEvents(Guid streamId)
        {
            _logger.Debug($"Loading entire event stream for {streamId}");

            var doesStreamExist = await _repository
                .CheckStream(streamId)
                .ConfigureAwait(false);

            if (!doesStreamExist)
                return null;

            _logger.Debug($"Get all stored events for stream {streamId}");

            var storedEvents = await _repository
                .GetAll(streamId)
                .ConfigureAwait(false);

            _logger.Debug("Convert all stored events to domain events");

            var domainEvents = storedEvents?
                .Select(GetDomainEvent)
                .ToList() ?? new List<DomainEvent>();

            _logger.Debug($"Returning all domain events for stream {streamId}");

            return domainEvents;
        }

        private DomainEvent GetDomainEvent(StoredEvent storedEvent)
        {
            var deserialized = _serializer
                .Deserialize(storedEvent.EventBody, storedEvent.EventType);

            return deserialized as DomainEvent;
        }
    }
}