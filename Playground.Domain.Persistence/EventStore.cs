using System;
using System.Collections.Generic;

namespace Playground.Domain.Persistence
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

        public void StoreEvents(
            Guid streamId,
            ICollection<IEvent> eventsToStore)
        {
            throw new NotImplementedException();
        }

        public ICollection<IEvent> LoadAllEvents(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public ICollection<IEvent> LoadSelectedEvents(
            Guid streamId,
            long fromEventId,
            long toEventId)
        {
            throw new NotImplementedException();
        }
    }
}