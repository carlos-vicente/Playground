using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Playground.Domain.Persistence.Events;

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

        private const string SelectLatestStreamSql = "SELECT \"EventStreamId\", \"EventStreamName\" FROM public.\"EventStreams\" ORDER BY \"CreatedOn\" DESC LIMIT 1";
        private const string SelectStreamEventsSql = "SELECT \"EventId\", \"TypeName\", \"OccurredOn\", \"BatchId\", \"EventBody\" FROM public.\"Events\"";
        private const string CreateEventStreamSql = "INSERT INTO public.\"EventStreams\" (\"EventStreamId\", \"EventStreamName\", \"CreatedOn\") values(@streamId, @streamName, @createdOn);";
        private const string CreateEventSql = "INSERT INTO public.\"Events\" (\"EventStreamId\", \"EventId\", \"TypeName\", \"OccurredOn\", \"BatchId\", \"EventBody\") values(@streamId, @eventId, @typeName, @occurredOn, @batchId, @body);";

        private const string DeleteEventsSql = "DELETE FROM public.\"Events\";";
        private const string DeleteEventStreamsSql = "DELETE FROM public.\"EventStreams\";";

        public static void CleanEvents()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DeleteEventsSql;
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public static void CleanEventStreams()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DeleteEventStreamsSql;
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
                    .QueryAsync<EventStream>(SelectLatestStreamSql)
                    .ConfigureAwait(false);

                return query.Any()
                    ? query.Single()
                    : default(EventStream);
            }
        }

        public static async Task CreateEventStream(Guid streamId, string streamName)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = CreateEventStreamSql;
                    command.Parameters.AddWithValue("@streamId", streamId);
                    command.Parameters.AddWithValue("@streamName", streamName);
                    command.Parameters.AddWithValue("@createdOn", DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task CreateEvent(Guid streamId, StoredEvent storedEvent)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                await connection
                    .OpenAsync()
                    .ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = CreateEventSql;

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

        public static async Task CreateEvents(Guid streamId, IEnumerable<StoredEvent> storedEvents)
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
                        command.CommandText = CreateEventSql;

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
                    .QueryAsync<StoredEvent>(SelectStreamEventsSql)
                    .ConfigureAwait(false);

                return query.ToList();
            }
        }
    }
}