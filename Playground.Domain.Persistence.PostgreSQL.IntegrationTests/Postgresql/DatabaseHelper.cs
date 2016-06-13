using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Playground.Domain.Persistence.PostgreSQL.IntegrationTests.Postgresql
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

        private const string SelectLatestStreamSql = "SELECT \"EventStreamId\", \"CreatedOn\" FROM public.\"EventStreams\" ORDER BY \"CreatedOn\" DESC LIMIT 1";
        private const string CreateEventStreamSql = "INSERT INTO public.\"EventStreams\" (\"EventStreamId\", \"CreatedOn\") values(@streamId, @createdOn);";
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

        public static async Task<Guid> GetLatestStreamCreated()
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
                    ? query.Single().EventStreamId
                    : Guid.Empty;
            }
        }

        public static async Task CreateEventStream(Guid streamId)
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
                    command.Parameters.AddWithValue("@createdOn", DateTime.UtcNow);

                    await command
                        .ExecuteNonQueryAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        //public static void CreateTestTable()
        //{
        //    using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = Scripts.Ddl.CreateTable;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static void CreateStoredProcedure()
        //{
        //    using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = Scripts.Ddl.CreateProdecure;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static void DropTestTable()
        //{
        //    using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = Scripts.Ddl.DropTable;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static void DropStoredProcedure()
        //{
        //    using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = Scripts.Ddl.DropProcedure;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static void InsertData(Test data)
        //{
        //    using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
        //    {
        //        connection.Open();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = Scripts.Ddl.DropTable;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}