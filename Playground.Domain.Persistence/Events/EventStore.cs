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

        public EventStore(
            IEventSerializer serializer,
            IEventRepository repository)
        {
            _serializer = serializer;
            _repository = repository;
        }

        public async Task CreateEventStream(Guid streamId)
        {
            await _repository
                .CreateStream(streamId)
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

            var lastStoredEventId = 0L;
            if (lastStoredEvent != null)
                lastStoredEventId = lastStoredEvent.EventId;

            var events = eventsToStore
                .Select(e => new StoredEvent(
                    e.GetType().AssemblyQualifiedName,
                    e.Metadata.OccorredOn,
                    _serializer.Serialize(e as object),
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