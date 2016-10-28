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
        protected NpgsqlConnectionStringBuilder ConnectionStringBuilder;

        static AggregateContextPerformanceTestBase()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();
        }

        public override void SetUp()
        {
            base.SetUp();

            ConnectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            DatabaseHelper.CleanEvents(ConnectionStringBuilder);
            DatabaseHelper.CleanEventStreams(ConnectionStringBuilder);

            var eventRepository = new EventRepository(ConnectionStringBuilder);

            var eventSerializer = new Serialization.Newtonsoft.EventSerializer();

            var logger = new SerilogLogger(Log.ForContext<EventStore>());

            var eventStore = new EventStore(
                eventSerializer,
                eventRepository,
                logger,
                Guid.NewGuid);

            AggregateContext = new AggregateContext(
                eventStore,
                null, // TODO: replace with actual SnapshotStore
                new AggregateHydrator(),
                new DummyDispatcher());
        }
    }
}
