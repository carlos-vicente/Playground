using Npgsql;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers
{
    public static class DatabaseHelper
    {
        private const string DeleteEventsSql = "DELETE FROM public.\"Events\";";
        private const string DeleteEventStreamsSql = "DELETE FROM public.\"EventStreams\";";

        public static void CleanEvents(NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder)
        {
            using (var connection = new NpgsqlConnection(npgsqlConnectionStringBuilder))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DeleteEventsSql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CleanEventStreams(NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder)
        {
            using (var connection = new NpgsqlConnection(npgsqlConnectionStringBuilder))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = DeleteEventStreamsSql;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}