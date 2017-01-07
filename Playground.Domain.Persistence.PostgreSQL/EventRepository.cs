using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Playground.Domain.Persistence.Events;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public class EventRepository : IEventRepository
    {
        private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

        static EventRepository()
        {
            NpgsqlConnection.MapCompositeGlobally<Event>("es.event");
        }

        public EventRepository(NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }

        public async Task CreateStream(Guid streamId, string streamName)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            if(string.IsNullOrWhiteSpace(streamName))
                throw new ArgumentException("Pass in a valid aggregate type name", nameof(streamName));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Commands.Scripts.CreateEventStream))
                {
                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Uuid, streamId);
                    command
                        .Parameters
                        .AddWithValue("streamname", NpgsqlDbType.Varchar, streamName);
                    command
                        .Parameters
                        .AddWithValue("createdon", NpgsqlDbType.Timestamp, DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<bool> CheckStream(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                bool streamExists = false;
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Queries.Scripts.CheckIfStreamExists))
                {
                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Uuid, streamId);

                    streamExists = (bool)(await command
                        .ExecuteScalarAsync()
                        .ConfigureAwait(false));
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
                return streamExists;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetAll(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var events = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.Scripts.GetAllEvents,
                        new {streamid = streamId},
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .OrderBy(se => se.OccurredOn)
                    .ToArray();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return events;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetSelected(Guid streamId, long fromEventId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            if (fromEventId < 0L)
                throw new ArgumentException("Pass in a valid event identifier, minimum value is 0", nameof(fromEventId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var events = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.Scripts.GetSelectedEvents,
                        new { streamid = streamId, fromeventid = fromEventId },
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .OrderBy(se => se.OccurredOn)
                    .ToArray();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return events;
            }
        }

        public Task<StoredEvent> Get(Guid streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<StoredEvent> GetLast(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var lastEvent = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.Scripts.GetLastEvent,
                        new {streamid = streamId},
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .SingleOrDefault();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return lastEvent;
            }
        }

        public Task Add(Guid streamId, StoredEvent storedEvent)
        {
            throw new NotImplementedException();
        }

        public async Task Add(Guid streamId, ICollection<StoredEvent> events)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            if (events == null || !events.Any())
                throw new ArgumentException("Must have at least one event", nameof(events));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Commands.Scripts.AddEvents))
                {
                    var eventArray = events
                        .Select(e => new Event
                        {
                            eventid = e.EventId,
                            occurredon = e.OccurredOn,
                            typename = e.TypeName,
                            batchid = e.BatchId,
                            eventbody = e.EventBody
                        })
                        .ToArray();

                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Uuid, streamId);
                    command
                        .Parameters
                        .AddWithValue("events", NpgsqlDbType.Array | NpgsqlDbType.Composite, eventArray);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
            }
        }

        public Task Remove(Guid streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Guid streamId)
        {
            throw new NotImplementedException();
        }

        private NpgsqlCommand CreateStoredProcedureCommand(
            NpgsqlConnection connection,
            string storedProcedure)
        {
            return new NpgsqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private async Task<NpgsqlConnection> OpenConnection()
        {
            var conn = new NpgsqlConnection(_connectionStringBuilder);
            await conn
                .OpenAsync()
                .ConfigureAwait(false);
            return conn;
        }
    }

    public class EventRepositoryForGenericIdentity : IEventRepositoryForGenericIdentity
    {
        private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

        static EventRepositoryForGenericIdentity()
        {
            NpgsqlConnection.MapCompositeGlobally<Event>("esgeneric.event");
        }

        public EventRepositoryForGenericIdentity(NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }

        public async Task CreateStream(string streamId, string streamName)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            if (string.IsNullOrWhiteSpace(streamName))
                throw new ArgumentException("Pass in a valid aggregate type name", nameof(streamName));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Commands.ScriptsWithString.CreateEventStream))
                {
                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Varchar, streamId);
                    command
                        .Parameters
                        .AddWithValue("streamname", NpgsqlDbType.Varchar, streamName);
                    command
                        .Parameters
                        .AddWithValue("createdon", NpgsqlDbType.Timestamp, DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<bool> CheckStream(string streamId)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                bool streamExists = false;
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Queries.ScriptsWithString.CheckIfStreamExists))
                {
                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Varchar, streamId);

                    streamExists = (bool)(await command
                        .ExecuteScalarAsync()
                        .ConfigureAwait(false));
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
                return streamExists;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetAll(string streamId)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var events = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.ScriptsWithString.GetAllEvents,
                        new { streamid = streamId },
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .OrderBy(se => se.OccurredOn)
                    .ToArray();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return events;
            }
        }

        public async Task<IEnumerable<StoredEvent>> GetSelected(string streamId, long fromEventId)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            if (fromEventId < 0L)
                throw new ArgumentException("Pass in a valid event identifier, minimum value is 0", nameof(fromEventId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var events = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.ScriptsWithString.GetSelectedEvents,
                        new { streamid = streamId, fromeventid = fromEventId },
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .OrderBy(se => se.OccurredOn)
                    .ToArray();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return events;
            }
        }

        public Task<StoredEvent> Get(string streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<StoredEvent> GetLast(string streamId)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                var lastEvent = (await connection
                    .QueryAsync<StoredEvent>(
                        Queries.ScriptsWithString.GetLastEvent,
                        new { streamid = streamId },
                        trans,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false))
                    .SingleOrDefault();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return lastEvent;
            }
        }

        public Task Add(string streamId, StoredEvent storedEvent)
        {
            throw new NotImplementedException();
        }

        public async Task Add(string streamId, ICollection<StoredEvent> events)
        {
            if (string.IsNullOrWhiteSpace(streamId))
                throw new ArgumentException("Pass in a valid stream id", nameof(streamId));

            if (events == null || !events.Any())
                throw new ArgumentException("Must have at least one event", nameof(events));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (var command = CreateStoredProcedureCommand(
                    connection,
                    Commands.ScriptsWithString.AddEvents))
                {
                    var eventArray = events
                        .Select(e => new Event
                        {
                            eventid = e.EventId,
                            occurredon = e.OccurredOn,
                            typename = e.TypeName,
                            batchid = e.BatchId,
                            eventbody = e.EventBody
                        })
                        .ToArray();

                    command
                        .Parameters
                        .AddWithValue("streamid", NpgsqlDbType.Varchar, streamId);
                    command
                        .Parameters
                        .AddWithValue("events", NpgsqlDbType.Array | NpgsqlDbType.Composite, eventArray);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);
            }
        }

        public Task Remove(string streamId, long eventId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string streamId)
        {
            throw new NotImplementedException();
        }

        private NpgsqlCommand CreateStoredProcedureCommand(
            NpgsqlConnection connection,
            string storedProcedure)
        {
            return new NpgsqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private async Task<NpgsqlConnection> OpenConnection()
        {
            var conn = new NpgsqlConnection(_connectionStringBuilder);
            await conn
                .OpenAsync()
                .ConfigureAwait(false);
            return conn;
        }
    }
}
