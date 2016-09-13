using System;
using System.Configuration;
using DbUp;

namespace Playground.TicketOffice.SqlServer.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["MovieTicketOfficeDb"]
                .ConnectionString;

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsFromFileSystem("./Scripts")
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
        }
    }
}
