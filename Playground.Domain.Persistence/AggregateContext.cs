using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.Snapshots;
using Playground.Messaging;

namespace Playground.Domain.Persistence
{
    public class AggregateContext : IAggregateContext
    {
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IAggregateHydrator _aggregateHydrator;
        private readonly IEventDispatcher _eventDispatcher;

        public AggregateContext(
            IEventStore eventStore,
            ISnapshotStore snapshotStore,
            IAggregateHydrator aggregateHydrator,
            IEventDispatcher eventDispatcher)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _aggregateHydrator = aggregateHydrator;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot, TAggregateState>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            //TODO: access if we should create a new stream at this point.
            // What if something goes wrong when processing the command that creates this,
            // then we would have an empty stream
            // If this should be kept, then the TryLoad and Load method must be able to return
            // an aggregate when there are no events, but there is a record created for it

            await _eventStore
                .CreateEventStream<TAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            return GetAggregateInstance<TAggregateRoot, TAggregateState>(aggregateRootId, null, null);
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot, TAggregateState>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            var snapshot = await _snapshotStore
                .GetLastestSnaptshot<TAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            ICollection<DomainEvent> events;

            if (snapshot != null)
            {
                events = await _eventStore
                    .LoadSelectedEvents(aggregateRootId, snapshot.Version)
                    .ConfigureAwait(false);
            }
            else
            {
                events = await _eventStore
                    .LoadAllEvents(aggregateRootId)
                    .ConfigureAwait(false);
            }

            return events == null
                ? null
                : GetAggregateInstance<TAggregateRoot, TAggregateState>(
                    aggregateRootId,
                    events,
                    snapshot?.Data);
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            // TODO: check if the is a snapshot available for this stream
            // TODO: if there is a snapshot, then only obtain the events after the snapshot's version

            var events = await _eventStore
                .LoadAllEvents(aggregateRootId)
                .ConfigureAwait(false);

            if (events == null)
            {
                throw new InvalidOperationException($"No stream with identifier {aggregateRootId}");
            }

            return GetAggregateInstance<TAggregateRoot, TAggregateState>(aggregateRootId, events, null);
        }

        public async Task Save<TAggregateRoot, TAggregateState>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            await _eventStore
                .StoreEvents(
                    aggregateRoot.Id,
                    aggregateRoot.CurrentVersion,
                    aggregateRoot.UncommittedEvents)
                .ConfigureAwait(false);

            // everything worked correctly so lets dispatch the events
            foreach (var domainEvent in aggregateRoot.UncommittedEvents)
            {
                await _eventDispatcher
                    .RaiseEvent(domainEvent)
                    .ConfigureAwait(false);
            }
        }

        private TAggregateRoot GetAggregateInstance<TAggregateRoot, TAggregateState>(
            Guid aggregateRootId,
            ICollection<DomainEvent> events,
            TAggregateState snapshotData)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            TAggregateRoot instance;

            if (events != null && events.Any())
            {
                var state = _aggregateHydrator
                    .HydrateAggregateWithEvents<TAggregateState>(events, snapshotData);

                var currentVersion = events.Last().Metadata.StorageVersion;

                instance = Activator
                    .CreateInstance(typeof(TAggregateRoot), aggregateRootId, state, currentVersion) as TAggregateRoot;
            }
            else
            {
                instance = Activator
                    .CreateInstance(typeof(TAggregateRoot), aggregateRootId) as TAggregateRoot;
            }

            return instance;
        }
    }
}