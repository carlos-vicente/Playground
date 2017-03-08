using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.Snapshots;

namespace Playground.Domain.Persistence.PostgreSQL.TestsHelper
{
    public static class DatabaseHelper
    {
        public static NpgsqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
        }

        private const string GuidIdSchema = "ES";
        private const string StringIdSchema = "ESGeneric";

        private const string SelectLatestStreamSql = "SELECT \"EventStreamId\", \"EventStreamName\" FROM {0}.\"EventStreams\" ORDER BY \"CreatedOn\" DESC LIMIT 1";
        private const string SelectStreamEventsSql = "SELECT \"EventId\", \"TypeName\", \"OccurredOn\", \"BatchId\", \"EventBody\" FROM {0}.\"Events\" WHERE \"EventStreamId\" = @streamId";
        private const string CreateEventStreamSql = "INSERT INTO {0}.\"EventStreams\" (\"EventStreamId\", \"EventStreamName\", \"CreatedOn\") values(@streamId, @streamName, @createdOn);";
        private const string CreateEventSql = "INSERT INTO {0}.\"Events\" (\"EventStreamId\", \"EventId\", \"TypeName\", \"OccurredOn\", \"BatchId\", \"EventBody\") values(@streamId, @eventId, @typeName, @occurredOn, @batchId, @body);";
        private const string CreateSnapshotSql = "INSERT INTO {0}.\"Snapshots\" (\"EventStreamId\", \"Version\", \"TakenOn\", \"Data\") values(@streamId, @version, @takenOn, @data);";

        private const string DeleteEventsSql = "DELETE FROM {0}.\"Events\";";
        private const string DeleteSnapshotsSql = "DELETE FROM {0}.\"Snapshots\";";
        private const string DeleteEventStreamsSql = "DELETE FROM {0}.\"EventStreams\";";

        #region guid id
        public static void CleanEventStreams()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteSnapshotsSql, GuidIdSchema);
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteEventsSql, GuidIdSchema);
                    command.ExecuteNonQuery();
                }
            
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteEventStreamsSql, GuidIdSchema);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task<EventStream> GetLatestStreamCreated()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                var query = await connection
                    .QueryAsync<EventStream>(string.Format(SelectLatestStreamSql, GuidIdSchema))
                    .ConfigureAwait(false);

                return query.Any()
                    ? query.Single()
                    : default(EventStream);
            }
        }

        public static async Task CreateEventStream(
            Guid streamId,
            string streamName)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CreateEventStreamSql, GuidIdSchema);
                    command.Parameters.AddWithValue("@streamId", streamId);
                    command.Parameters.AddWithValue("@streamName", streamName);
                    command.Parameters.AddWithValue("@createdOn", DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateEvent(
            Guid streamId,
            StoredEvent storedEvent)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CreateEventSql, GuidIdSchema);

                    command.Parameters.AddWithValue("@streamId", streamId);
                    command.Parameters.AddWithValue("@eventId", storedEvent.EventId);
                    command.Parameters.AddWithValue("@typeName", storedEvent.TypeName);
                    command.Parameters.AddWithValue("@occurredOn", storedEvent.OccurredOn);
                    command.Parameters.AddWithValue("@batchId", storedEvent.BatchId);
                    command.Parameters.AddWithValue("@body", NpgsqlDbType.Json, storedEvent.EventBody);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateSnapshot(
            Guid streamId,
            StoredSnapshot storedSnapshot)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CreateSnapshotSql, GuidIdSchema);

                    command.Parameters.AddWithValue("@streamId", streamId);
                    command.Parameters.AddWithValue("@version", storedSnapshot.Version);
                    command.Parameters.AddWithValue("@takenOn", storedSnapshot.TakenOn);
                    command.Parameters.AddWithValue("@data", NpgsqlDbType.Json, storedSnapshot.Data);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateEvents(
            Guid streamId,
            IEnumerable<StoredEvent> storedEvents)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                foreach (var storedEvent in storedEvents)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format(CreateEventSql, GuidIdSchema);

                        command.Parameters.AddWithValue("@streamId", streamId);
                        command.Parameters.AddWithValue("@eventId", storedEvent.EventId);
                        command.Parameters.AddWithValue("@typeName", storedEvent.TypeName);
                        command.Parameters.AddWithValue("@occurredOn", storedEvent.OccurredOn);
                        command.Parameters.AddWithValue("@batchId", storedEvent.BatchId);
                        command.Parameters.AddWithValue("@body", NpgsqlDbType.Json, storedEvent.EventBody);

                        await command
                            .ExecuteNonQueryAsync()
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        public static async Task<IEnumerable<StoredEvent>> GetStreamEvents(Guid streamId)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                var query = await connection
                    .QueryAsync<StoredEvent>(
                        string.Format(SelectStreamEventsSql, GuidIdSchema),
                        new
                        {
                            streamId
                        })
                    .ConfigureAwait(false);

                return query.ToList();
            }
        }
        #endregion

        #region string id
        public static void CleanEventStreamsGeneric()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteSnapshotsSql, StringIdSchema);
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteEventsSql, StringIdSchema);
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(DeleteEventStreamsSql, StringIdSchema);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task<EventStreamForGenericIdentity> GetLatestStreamCreatedGeneric()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                var query = await connection
                    .QueryAsync<EventStreamForGenericIdentity>(string.Format(SelectLatestStreamSql, StringIdSchema))
                    .ConfigureAwait(false);

                return query.Any()
                    ? query.Single()
                    : default(EventStreamForGenericIdentity);
            }
        }

        public static async Task CreateEventStreamGeneric(
            string streamId,
            string streamName)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CreateEventStreamSql, StringIdSchema);
                    command.Parameters.AddWithValue("@streamId", NpgsqlDbType.Varchar, streamId);
                    command.Parameters.AddWithValue("@streamName", streamName);
                    command.Parameters.AddWithValue("@createdOn", DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateEventGeneric(
            string streamId,
            StoredEvent storedEvent)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CreateEventSql, StringIdSchema);

                    command.Parameters.AddWithValue("@streamId", streamId);
                    command.Parameters.AddWithValue("@eventId", storedEvent.EventId);
                    command.Parameters.AddWithValue("@typeName", storedEvent.TypeName);
                    command.Parameters.AddWithValue("@occurredOn", storedEvent.OccurredOn);
                    command.Parameters.AddWithValue("@batchId", storedEvent.BatchId);
                    command.Parameters.AddWithValue("@body", NpgsqlDbType.Json, storedEvent.EventBody);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateEventsGeneric(
            string streamId,
            IEnumerable<StoredEvent> storedEvents)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                foreach (var storedEvent in storedEvents)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format(CreateEventSql, StringIdSchema);

                        command.Parameters.AddWithValue("@streamId", streamId);
                        command.Parameters.AddWithValue("@eventId", storedEvent.EventId);
                        command.Parameters.AddWithValue("@typeName", storedEvent.TypeName);
                        command.Parameters.AddWithValue("@occurredOn", storedEvent.OccurredOn);
                        command.Parameters.AddWithValue("@batchId", storedEvent.BatchId);
                        command.Parameters.AddWithValue("@body", NpgsqlDbType.Json, storedEvent.EventBody);

                        await command
                            .ExecuteNonQueryAsync()
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        public static async Task<IEnumerable<StoredEvent>> GetStreamEventsGeneric(
            string streamId)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                var query = await connection
                    .QueryAsync<StoredEvent>(
                        string.Format(SelectStreamEventsSql, StringIdSchema),
                        new
                        {
                            streamId
                        })
                    .ConfigureAwait(false);

                return query.ToList();
            }
        }
        #endregion
    }
}