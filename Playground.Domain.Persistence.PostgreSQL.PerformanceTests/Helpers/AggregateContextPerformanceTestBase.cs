using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Playground.Logging.Serilog;
using Playground.Tests;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers
{
    public abstract class AggregateContextPerformanceTestBase : SimpleTestBase
    {
        private IEventSerializer EventSerializer;
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

            DatabaseHelper.CleanEvents();
            DatabaseHelper.CleanEventStreams();

            var eventRepository = new EventRepository(DatabaseHelper.GetConnectionStringBuilder());

            EventSerializer = new Serialization.Newtonsoft.EventSerializer();

            var logger = new SerilogLogger(Log.ForContext<EventStore>());

            var eventStore = new EventStore(
                EventSerializer,
                eventRepository,
                logger,
                Guid.NewGuid);

            AggregateContext = new AggregateContext(
                eventStore,
                null, // TODO: replace with actual SnapshotStore
                new AggregateHydrator(),
                new DummyDispatcher());
        }

        protected IEnumerable<StoredEvent> GetStoredEvents(IEnumerable<DomainEvent> domainEvents)
        {
            if (domainEvents == null)
            {
                return null;
            }

            var batchId = Guid.NewGuid();
            var lastStoredEventId = 0L;

            return domainEvents
                .Select(de => new StoredEvent(
                    de.GetType().AssemblyQualifiedName,
                    de.Metadata.OccorredOn,
                    EventSerializer.Serialize(de),
                    batchId,
                    ++lastStoredEventId))
                .ToList();
        }
    }
}
