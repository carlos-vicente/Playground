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
                    .LoadSelectedEvents(aggregateRootId, snapshot.Version, long.MaxValue)
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
                    snapshot);
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState>(Guid aggregateRootId)
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
                    .LoadSelectedEvents(
                        aggregateRootId,
                        snapshot.Version + 1,
                        long.MaxValue)
                    .ConfigureAwait(false);
            }
            else
            {
                events = await _eventStore
                    .LoadAllEvents(aggregateRootId)
                    .ConfigureAwait(false);
            }

            if (snapshot == null && events == null)
            {
                throw new InvalidOperationException($"No stream with identifier {aggregateRootId}");
            }

            return GetAggregateInstance<TAggregateRoot, TAggregateState>(
                aggregateRootId,
                events,
                snapshot);
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
            Snapshot<TAggregateState> snapshot)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            TAggregateRoot instance;

            if ((events != null && events.Any()) || snapshot != null)
            {
                var state = _aggregateHydrator
                    .HydrateAggregateWithEvents<TAggregateState>(
                        events ?? new List<DomainEvent>(),
                        snapshot?.Data);

                var currentVersion = events != null && events.Any()
                    ? events.Last().Metadata.Version
                    : snapshot?.Version ?? 0;

                instance = Activator
                        .CreateInstance(
                            typeof(TAggregateRoot),
                            aggregateRootId,
                            state,
                            currentVersion)
                    as TAggregateRoot;
            }
            else
            {
                instance = Activator
                    .CreateInstance(typeof(TAggregateRoot), aggregateRootId) as TAggregateRoot;
            }

            return instance;
        }
    }

    public class AggregateContextForAggregateWithIdentity : IAggregateContextForAggregateWithIdentity
    {
        private readonly IEventStoreForGenericIdentity _eventStore;
        private readonly ISnapshotStoreWithGenericIdentity _snapshotStore;
        private readonly IAggregateHydratorWithGenericIdentity _aggregateHydrator;
        private readonly IEventDispatcherWithGenericIdentity _eventDispatcher;

        public AggregateContextForAggregateWithIdentity(
            IEventStoreForGenericIdentity eventStore,
            ISnapshotStoreWithGenericIdentity snapshotStore,
            IAggregateHydratorWithGenericIdentity aggregateHydrator,
            IEventDispatcherWithGenericIdentity eventDispatcher)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _aggregateHydrator = aggregateHydrator;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId) 
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity> 
            where TAggregateState : class, IAggregateState, new() 
            where TIdentity : IIdentity
        {
            //TODO: access if we should create a new stream at this point.
            // What if something goes wrong when processing the command that creates this,
            // then we would have an empty stream
            // If this should be kept, then the TryLoad and Load method must be able to return
            // an aggregate when there are no events, but there is a record created for it

            await _eventStore
                .CreateEventStream<TAggregateRoot>(aggregateRootId.Id)
                .ConfigureAwait(false);

            return GetAggregateInstance<TAggregateRoot, TAggregateState, TIdentity>(
                aggregateRootId,
                null,
                null);
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            var snapshot = await _snapshotStore
                .GetLastestSnaptshot<TAggregateState, TIdentity>(aggregateRootId)
                .ConfigureAwait(false);

            ICollection<DomainEventForAggregateRootWithIdentity> events;

            if (snapshot != null)
            {
                events = await _eventStore
                    .LoadSelectedEvents(aggregateRootId.Id, snapshot.Version, long.MaxValue)
                    .ConfigureAwait(false);
            }
            else
            {
                events = await _eventStore
                    .LoadAllEvents(aggregateRootId.Id)
                    .ConfigureAwait(false);
            }

            return events == null
                ? null
                : GetAggregateInstance<TAggregateRoot, TAggregateState, TIdentity>(
                    aggregateRootId,
                    events,
                    snapshot);
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            var snapshot = await _snapshotStore
                .GetLastestSnaptshot<TAggregateState, TIdentity>(aggregateRootId)
                .ConfigureAwait(false);

            ICollection<DomainEventForAggregateRootWithIdentity> events;

            if (snapshot != null)
            {
                events = await _eventStore
                    .LoadSelectedEvents(aggregateRootId.Id, snapshot.Version, long.MaxValue)
                    .ConfigureAwait(false);
            }
            else
            {
                events = await _eventStore
                    .LoadAllEvents(aggregateRootId.Id)
                    .ConfigureAwait(false);
            }

            if (snapshot == null && events == null)
            {
                throw new InvalidOperationException($"No stream with identifier {aggregateRootId}");
            }

            return GetAggregateInstance<TAggregateRoot, TAggregateState, TIdentity>(
                aggregateRootId,
                events,
                snapshot);
        }

        public async Task Save<TAggregateRoot, TAggregateState, TIdentity>(
            TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            await _eventStore
                .StoreEvents(
                    aggregateRoot.Identity.Id,
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

        private TAggregateRoot GetAggregateInstance<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId,
            ICollection<DomainEventForAggregateRootWithIdentity> events,
            Snapshot<TAggregateState> snapshot)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            TAggregateRoot instance;

            if ((events != null && events.Any()) || snapshot != null)
            {
                var state = _aggregateHydrator
                    .HydrateAggregateWithEvents<TAggregateState>(
                        events ?? new List<DomainEventForAggregateRootWithIdentity>(),
                        snapshot?.Data);

                var currentVersion = events != null && events.Any()
                    ? events.Last().Metadata.Version
                    : snapshot?.Version ?? 0;

                instance = Activator
                        .CreateInstance(
                            typeof(TAggregateRoot),
                            aggregateRootId,
                            state,
                            currentVersion)
                    as TAggregateRoot;
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