using System;
using System.Configuration;
using System.Reflection;
using DbUp;
using Npgsql;

namespace Playground.Domain.Persistence.PostgreSQL.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var databaseName = args.Length > 0
                ? args[0]
                : null;

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = databaseName ?? ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            }
            .ConnectionString;

            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                Console.ReadLine();
            }
        }
    }
}
