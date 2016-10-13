using System;
using System.Configuration;
using Npgsql;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Logging.Serilog;
using Playground.Tests;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers
{
    public abstract class AggregateContextPerformanceTestBase : SimpleTestBase
    {
        protected IAggregateContext AggregateContext;

        static AggregateContextPerformanceTestBase()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();
        }

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

            DatabaseHelper.CleanEvents(connectionStringBuilder);
            DatabaseHelper.CleanEventStreams(connectionStringBuilder);

            var eventRepository = new EventRepository(connectionStringBuilder);

            var eventSerializer = new Serialization.Newtonsoft.EventSerializer();

            var logger = new SerilogLogger(Log.ForContext<EventStore>());

            var eventStore = new EventStore(
                eventSerializer,
                eventRepository,
                logger,
                Guid.NewGuid);

            AggregateContext = new AggregateContext(
                eventStore,
                new AggregateHydrator(),
                new DummyDispatcher());
        }
    }
}
