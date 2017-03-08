using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Playground.Core.Serialization;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Playground.Domain.Persistence.Snapshots;
using Playground.Logging.Serilog;
using Playground.Tests;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Helpers
{
    public abstract class AggregateContextPerformanceTestBase : SimpleTestBase
    {
        protected static double MaximumAcceptedDuration = 1000;

        protected IObjectSerializer ObjectSerializer;

        protected IAggregateContext AggregateContext;

        protected NpgsqlConnectionStringBuilder ConnectionStringBuilder;

        protected IMetricsCounter MetricsCounter => AggregateContext as IMetricsCounter;

        static AggregateContextPerformanceTestBase()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();
        }

        public override void SetUp()
        {
            base.SetUp();

            DatabaseHelper.CleanEventStreams();

            var connectionStringBuilder = DatabaseHelper.GetConnectionStringBuilder();

            var eventRepository = new EventRepository(connectionStringBuilder);

            ObjectSerializer = new Serialization.Newtonsoft.ObjectSerializer();

            var logger = new SerilogLogger(Log.ForContext<EventStore>());

            var eventStore = new EventStore(
                ObjectSerializer,
                eventRepository,
                logger,
                Guid.NewGuid);

            var snapshotStore = new SnapshotStore(
                new SnapshotRepository(connectionStringBuilder),
                ObjectSerializer,
                logger);

            var actualAggregateContext = new AggregateContext(
                eventStore,
                snapshotStore,
                new AggregateHydrator(),
                new DummyDispatcher());

            AggregateContext = new AggregateContextMetricsCounter(actualAggregateContext);

            // warm up
            AggregateContext.TryLoad<Order, OrderState>(Guid.NewGuid());
        }
        
        protected IEnumerable<StoredEvent> GetStoredEvents(
            Guid aggregateId,
            IEnumerable<DomainEvent> domainEvents)
        {
            if (domainEvents == null)
            {
                return null;
            }

            var batchId = Guid.NewGuid();
            var lastStoredEventId = 0L;

            return domainEvents
                .Select(de =>
                {
                    de.Metadata = new Metadata(aggregateId, ++lastStoredEventId, de.GetType());

                    return new StoredEvent(
                        de.Metadata.EventType.AssemblyQualifiedName,
                        de.Metadata.OccorredOn,
                        ObjectSerializer.Serialize(de),
                        batchId,
                        de.Metadata.Version
                        );
                })
                .ToList();
        }

        protected StoredSnapshot GetStoredSnapshot<TSnapshotData>(
            long version,
            TSnapshotData data)
        {
            return new StoredSnapshot
            {
                Version = version,
                TakenOn = DateTime.UtcNow,
                Data = ObjectSerializer.Serialize<TSnapshotData>(data)
            };
        }
    }
}
