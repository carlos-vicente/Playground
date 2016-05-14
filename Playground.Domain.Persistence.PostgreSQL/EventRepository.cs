using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Core.Validation;
using Playground.Data.Contracts;
using Playground.Domain.Persistence.Events;
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
        public async Task CreateStream(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = new CreateEventStreamCommand
                {
                    streamId = streamId
                };

                await connection
                    .ExecuteCommandAsStoredProcedure(
                        Commands.Scripts.CreateEventStream,
                        command)
                    .ConfigureAwait(false);
            }
        }

        public async Task<bool> CheckStream(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new CheckIfStreamExistsQuery
                {
                    streamId = streamId
                };

                return await connection
                    .ExecuteQuerySingleAsStoredProcedure<bool>(
                        Queries.Scripts.CheckIfStreamExists,
                        query)
                    .ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetAll(Guid streamId)
        {
            if(streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetAllEventsQuery
                {
                    streamId = streamId
                };

                return await connection
                    .ExecuteQueryMultipleAsStoredProcedure<StoredEvent>(
                        Queries.Scripts.GetAllEvents,
                        query)
                    .ConfigureAwait(false);
            }
        }

        public async Task<StoredEvent> Get(Guid streamId, long eventId)
        {
            if (streamId == default(Guid))
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
                    .ExecuteQuerySingle<StoredEvent>(
                        Queries.Scripts.GetEvent,
                        query)
                    .ConfigureAwait(false);
            }
        }

        public async Task<StoredEvent> GetLast(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    streamId = streamId,
                };

                return await connection
                    .ExecuteQuerySingleAsStoredProcedure<StoredEvent>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);
            }
        }
        
        public async Task Add(Guid streamId, StoredEvent storedEvent)
        {
            if(streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            //var validator = _validatorFactory.CreateValidator<StoredEvent>();

            //validator.Validate(storedEvent);

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    streamId = streamId
                };

                var lastEvent = await connection
                    .ExecuteQuerySingle<StoredEvent>(
                        Queries.Scripts.GetLastEvent,
                        query)
                    .ConfigureAwait(false);

                if (storedEvent.EventId <= lastEvent.EventId)
                {
                    throw new InvalidOperationException(
                        $"New EventId {storedEvent.EventId} must be bigger than last EventId stored {lastEvent.EventId}");
                }

                //TODO: convert this in an array with 1 element

                //var command = new AddEventsCommand
                //{
                //    StreamId = streamId,
                //    EventId = storedEvent.EventId,
                //    EventBody = storedEvent.EventBody,
                //    OccurredOn = storedEvent.OccurredOn,
                //    TypeName = storedEvent.TypeName
                //};

                //await connection
                //    .ExecuteCommand(Commands.Scripts.AddEvents, command)
                //    .ConfigureAwait(false);
            }
        }

        public async Task Add(Guid streamId, ICollection<StoredEvent> events)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            //var validator = _validatorFactory.CreateValidator<StoredEvent>();

            //validator.ValidateAll(events);

            using (var connection = _connectionFactory.CreateConnection())
            {
                // TODO: remove this bit completely as it is EventStore's responsability

                //var query = new GetLastEventQuery
                //{
                //    streamId = streamId
                //};

                //var lastEvent = await connection
                //    .ExecuteQuerySingleAsStoredProcedure<StoredEvent>(
                //        Queries.Scripts.GetLastEvent,
                //        query)
                //    .ConfigureAwait(false);

                //var orderedEvents = events
                //    .OrderBy(e => e.EventId)
                //    .ToList();

                //if (orderedEvents.First().EventId <= lastEvent.EventId)
                //{
                //    throw new InvalidOperationException(
                //        $"New EventId {orderedEvents.First().EventId} must be bigger than last EventId stored {lastEvent.EventId}");
                //}

                //foreach (var @event in orderedEvents)
                //{
                //var command = new AddEventsCommand
                //{
                //    StreamId = streamId,
                //    EventId = @event.EventId,
                //    EventBody = @event.EventBody,
                //    OccurredOn = @event.OccurredOn,
                //    TypeName = @event.TypeName
                //};

                //await connection
                //    .ExecuteCommand(
                //        Commands.Scripts.AddEvents,
                //        command)
                //    .ConfigureAwait(false);
                //}

                var command = new AddEventsCommand
                {
                    streamId = streamId,
                    events = events
                                .Select(se => new AddEventsCommand.Event
                                {
                                    EventId = se.EventId,
                                    EventBody = se.EventBody,
                                    TypeName = se.TypeName,
                                    OccurredOn = se.OccurredOn
                                })
                                .ToArray()
                };

                await connection
                    .ExecuteCommandAsStoredProcedure(
                        Commands.Scripts.AddEvents,
                        command)
                    .ConfigureAwait(false);
            }
        }

        public async Task Remove(Guid streamId, long eventId)
        {
            if (streamId == default(Guid))
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
                    .ExecuteQuerySingle<StoredEvent>(
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
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", "streamId");

            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = new GetLastEventQuery
                {
                    streamId = streamId
                };

                var eventToDelete = await connection
                    .ExecuteQuerySingle<StoredEvent>(
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
