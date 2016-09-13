using System.Configuration;
using Autofac;
using Npgsql;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class EventStoreConnectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            builder.RegisterInstance(connectionStringBuilder);
        }
    }
}