using System.Configuration;
using Npgsql;

namespace Playground.Data.Dapper.Tests.Postgresql
{
    public static class DatabaseHelper
    {
        public static string GetConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"]
            };

            return builder.ConnectionString;
        }

        public static void CreateTestTable()
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
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
            

        }
    }
}