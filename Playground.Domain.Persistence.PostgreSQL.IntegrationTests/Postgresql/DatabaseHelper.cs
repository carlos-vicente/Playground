using System.Configuration;
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