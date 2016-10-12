using System;
using System.Configuration;
using Npgsql;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.Serialization.Jil;
using Playground.Tests;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public class AggregateContextPerformanceTestBase : SimpleTestBase
    {
        protected IAggregateContext AggregateContext;

        public override void SetUp()
        {
            base.SetUp();

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            var eventRepository = new EventRepository(connectionStringBuilder);

            var eventStore = new EventStore(new EventSerializer(), eventRepository, Guid.NewGuid);

            AggregateContext = new AggregateContext(eventStore, new AggregateHydrator(), new DummyDispatcher());
        }
    }
}
