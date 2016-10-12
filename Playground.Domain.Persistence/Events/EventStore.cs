using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.Events
{
    public class EventStore : IEventStore
    {
        private readonly IEventSerializer _serializer;
        private readonly IEventRepository _repository;
        private readonly Func<Guid> _batchIdProviderFunc;

        public EventStore(
            IEventSerializer serializer,
            IEventRepository repository,
            Func<Guid> batchIdProviderFunc)
        {
            _serializer = serializer;
            _repository = repository;
            _batchIdProviderFunc = batchIdProviderFunc;
        }

        public async Task CreateEventStream<TAggregateRoot>(Guid streamId)
        {
            var doesStreamAlreadyExists = await _repository
                .CheckStream(streamId)
                .ConfigureAwait(false);

            if(doesStreamAlreadyExists)
                throw new InvalidOperationException($"Stream with id {streamId} already exists!");

            await _repository
                .CreateStream(streamId, typeof(TAggregateRoot).AssemblyQualifiedName)
                .ConfigureAwait(false);
        }

        public async Task StoreEvents(
            Guid streamId,
            long currentVersion,
            ICollection<DomainEvent> eventsToStore)
        {
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

            var events = eventsToStore
                .Select(e => new StoredEvent(
                    e.GetType().AssemblyQualifiedName,
                    e.Metadata.OccorredOn,
                    _serializer.Serialize(e as object),
                    batchId,
                    ++lastStoredEventId))
                .ToList();

            await _repository
                .Add(streamId, events)
                .ConfigureAwait(false);
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
            var doesStreamExist = await _repository
                .CheckStream(streamId)
                .ConfigureAwait(false);

            if (!doesStreamExist)
                return null;

            var storedEvents = await _repository
                .GetAll(streamId)
                .ConfigureAwait(false);

            return storedEvents?
                .Select(GetDomainEvent)
                .ToList() ?? new List<DomainEvent>();
        }

        private DomainEvent GetDomainEvent(StoredEvent storedEvent)
        {
            //TODO: i need to send the type for deserialization!!!! -> storedEvent.EventType

            return _serializer
                .Deserialize(storedEvent.EventBody) as DomainEvent;
        }
    }
}