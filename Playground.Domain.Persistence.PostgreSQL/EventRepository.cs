using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Core.Validation;
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
        private readonly IValidatorFactory _validatorFactory;

        public EventRepository(
            IConnectionFactory connectionFactory,
            IValidatorFactory validatorFactory)
        {
            _connectionFactory = connectionFactory;
            _validatorFactory = validatorFactory;
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
                    .ExecuteQueryMultiple<Event>(
                        Queries.Scripts.GetAllEvents,
                        query)
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
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetEvent,
                        query)
                    .ConfigureAwait(false);
            }
        }

        public async Task<Event> GetLastEvent(Guid streamId)
        {
            if (streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    StreamId = streamId,
                };

                return await connection
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);
            }
        }

        public async Task Add(Guid streamId, Event @event)
        {
            if(streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            var validator = _validatorFactory.CreateValidator<Event>();
            validator.Validate(@event);

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    StreamId = streamId
                };

                var lastEvent = await connection
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);

                if (@event.EventId <= lastEvent.EventId)
                {
                    throw new InvalidOperationException(
                        $"New EventId {@event.EventId} must be bigger than last EventId stored {lastEvent.EventId}");
                }

                var command = new AddEventCommand
                {
                    StreamId = streamId,
                    EventId = @event.EventId,
                    EventBody = @event.EventBody,
                    OccurredOn = @event.OccurredOn,
                    TypeName = @event.TypeName
                };

                await connection
                    .ExecuteCommand(Commands.Scripts.AddEvent, command)
                    .ConfigureAwait(false);
            }
        }

        public async Task Add(Guid streamId, ICollection<Event> events)
        {
            if (streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            var validator = _validatorFactory.CreateValidator<Event>();
            validator.ValidateAll(events);

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    StreamId = streamId
                };

                var lastEvent = await connection
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);

                var orderedEvents = events
                    .OrderBy(e => e.EventId)
                    .ToList();

                if (orderedEvents.First().EventId <= lastEvent.EventId)
                {
                    throw new InvalidOperationException(
                        $"New EventId {orderedEvents.First().EventId} must be bigger than last EventId stored {lastEvent.EventId}");
                }

                foreach (var @event in orderedEvents)
                {
                    var command = new AddEventCommand
                    {
                        StreamId = streamId,
                        EventId = @event.EventId,
                        EventBody = @event.EventBody,
                        OccurredOn = @event.OccurredOn,
                        TypeName = @event.TypeName
                    };

                    await connection
                        .ExecuteCommand(
                            Commands.Scripts.AddEvent,
                            command)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task Remove(Guid streamId, long eventId)
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

                var eventToDelete = await connection
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetEvent,
                        query)
                    .ConfigureAwait(false);

                if (eventToDelete != null)
                {
                    var command = new RemoveEventCommand
                    {
                        StreamId = streamId,
                        EventId = eventId
                    };

                    await connection
                        .ExecuteCommand(
                            Commands.Scripts.RemoveEvent,
                            command)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task Remove(Guid streamId)
        {
            if (streamId == Guid.Empty)
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    StreamId = streamId
                };

                var eventToDelete = await connection
                    .ExecuteQuerySingle<Event>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);

                if (eventToDelete != null)
                {
                    var command = new RemoveAllEventsCommand
                    {
                        StreamId = streamId
                    };

                    await connection
                        .ExecuteCommand(
                            Commands.Scripts.RemoveAllEvents,
                            command)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
