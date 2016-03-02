using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.Data.Contracts;
using Playground.Domain.Persistence.PostgreSQL.Commands;
using Playground.Domain.Persistence.PostgreSQL.Queries;

namespace Playground.Domain.Persistence.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation for the IEventRepository
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public EventRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Event>> GetAll(Guid streamId)
        {
            if(streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetAllEventsQuery
                {
                    StreamId = streamId
                };

                return await connection
                    .ExecuteQueryMultiple<Event>("", query)
                    .ConfigureAwait(false);
            }
        }

        public async Task<Event> Get(Guid streamId, long eventId)
        {
            if (streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            if (eventId <= 0L)
                throw new ArgumentException("Pass in an event id greather than 0", "eventId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetEventQuery
                {
                    StreamId = streamId,
                    EventId = eventId
                };

                return await connection
                    .ExecuteQuerySingle<Event>("", query)
                    .ConfigureAwait(false);
            }
        }

        public async Task Add(Guid streamId, Event @event)
        {
            // TODO: sanity checks

            using (var connection = _connectionFactory.CreateConnection())
            {
                // TODO: make sure last event has lesser event id than current one

                var command = new AddEventCommand
                {
                    StreamId = streamId,
                    // TODO: add the remainder of the Event's properties
                };

                await connection
                    .ExecuteCommand("", command)
                    .ConfigureAwait(false);
            }
        }

        public Task Add(Guid streamId, IEnumerable<Event> events)
        {
            // make sure last event has lesser event id than the first one (considering the list is ordered by id)

            throw new NotImplementedException();
        }

        public Task Remove(Guid streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Guid streamId)
        {
            throw new NotImplementedException();
        }
    }
}
