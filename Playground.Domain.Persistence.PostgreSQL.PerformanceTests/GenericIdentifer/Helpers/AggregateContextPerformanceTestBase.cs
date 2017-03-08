using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Playground.Core.Serialization;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Playground.Domain.Persistence.Snapshots;
using Playground.Logging.Serilog;
using Playground.Tests;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers
{
    public abstract class AggregateContextPerformanceTestBase : SimpleTestBase
    {
        protected static double MaximumAcceptedDuration = 1000;

        protected IObjectSerializer ObjectSerializer;

        protected IAggregateContextForAggregateWithIdentity AggregateContext;

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

            var eventRepository = new EventRepositoryForGenericIdentity(connectionStringBuilder);

            ObjectSerializer = new Serialization.Newtonsoft.ObjectSerializer();

            var logger = new SerilogLogger(Log.ForContext<EventStore>());

            var eventStore = new EventStoreForGenericIdentity(
                ObjectSerializer,
                eventRepository,
                logger,
                Guid.NewGuid);

            var snapshotStore = new SnapshotStoreWithGenericIdentity(
                new SnapshotRepositoryWithGenericIdentity(connectionStringBuilder), 
                ObjectSerializer,
                logger);

            var actualAggregateContext = new AggregateContextForAggregateWithIdentity(
                eventStore,
                snapshotStore,
                new AggregateHydratorWithGenericIdentity(), 
                new DummyDispatcher());

            AggregateContext = new AggregateContextMetricsCounter(actualAggregateContext);

            // warm up
            AggregateContext.TryLoad<Order, OrderState, OrderIdentity>(new OrderIdentity(Guid.NewGuid()));
        }
        
        protected IEnumerable<StoredEvent> GetStoredEvents(
            string aggregateId,
            IEnumerable<DomainEventForAggregateRootWithIdentity> domainEvents)
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
                    de.Metadata = new MetadataForAggregateRootWithIdentity(
                        aggregateId,
                        ++lastStoredEventId,
                        de.GetType());

                    return new StoredEvent(
                        de.Metadata.EventType.AssemblyQualifiedName,
                        de.Metadata.OccorredOn,
                        ObjectSerializer.Serialize(de),
                        batchId,
                        de.Metadata.Version);
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
