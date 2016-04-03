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
                .Create(streamId)
                .ConfigureAwait(false);
        }

        public async Task StoreEvents(
            Guid streamId,
            long currentVersion,
            ICollection<IEvent> eventsToStore)
        {
            var lastStoredEvent = await _repository
                .GetLastEvent(streamId)
                .ConfigureAwait(false);

            // TODO: turn the if arround to throw exception
            if (currentVersion >= lastStoredEvent.EventId)
            {
                //var events = eventsToStore
                //    .Select(e => new StoredEvent(
                //        e.GetType().AssemblyQualifiedName,
                //        e.Metadata.OccorredOn,
                //        _serializer.Serialize(e),
                //        ))
            }
        }

        public Task<ICollection<IEvent>> LoadAllEvents(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<IEvent>> LoadSelectedEvents(
            Guid streamId,
            long fromEventId,
            long toEventId)
        {
            throw new NotImplementedException();
        }
    }
}