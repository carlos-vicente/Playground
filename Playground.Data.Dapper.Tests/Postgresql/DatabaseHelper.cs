using System.Configuration;
using Npgsql;

namespace Playground.Data.Dapper.Tests.Postgresql
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

                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
        }

        public static void CreateTestTable()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(Scripts.CreateTable, "test");
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DropTestTable()
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(Scripts.DropTable, "test");
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertData(Test data)
        {
            using (var connection = new NpgsqlConnection(GetConnectionStringBuilder()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(Scripts.DropTable, "test");
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}