using System;
using System.Collections.Generic;
using Playground.Data.Contracts;

namespace Playground.Domain.Persistence
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventSerializer _serializer;
        private readonly IConnectionFactory _connectionFactory;

        public SqlEventStore(
            IEventSerializer serializer,
            IConnectionFactory connectionFactory)
        {
            _serializer = serializer;
            _connectionFactory = connectionFactory;
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