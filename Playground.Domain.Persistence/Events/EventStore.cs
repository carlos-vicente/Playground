using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task<bool> CreateEventStream(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task StoreEvents(
            Guid streamId,
            ICollection<IEvent> eventsToStore)
        {
            throw new NotImplementedException();
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