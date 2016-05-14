using System.Configuration;
using System.Reflection;
using DbUp;

namespace Playground.Domain.Persistence.PostgreSQL.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
        }
    }
}
