using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Messaging;

namespace Playground.Domain.Persistence
{
    public class AggregateContext : IAggregateContext
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateHydrator _aggregateHydrator;
        private readonly IEventDispatcher _eventDispatcher;

        public AggregateContext(
            IEventStore eventStore,
            IAggregateHydrator aggregateHydrator,
            IEventDispatcher eventDispatcher)
        {
            _eventStore = eventStore;
            _aggregateHydrator = aggregateHydrator;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            await _eventStore
                .CreateEventStream(aggregateRootId)
                .ConfigureAwait(false);

            return GetAggregateInstance<TAggregateRoot>(aggregateRootId, null);
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            var events = await _eventStore
                .LoadAllEvents(aggregateRootId)
                .ConfigureAwait(false);

            return events == null 
                ? null 
                : GetAggregateInstance<TAggregateRoot>(aggregateRootId, events);
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            var events = await _eventStore
                .LoadAllEvents(aggregateRootId)
                .ConfigureAwait(false);

            if (events == null)
            {
                throw new InvalidOperationException(
                    $"No stream with identifier {aggregateRootId}");
            }

            return GetAggregateInstance<TAggregateRoot>(aggregateRootId, events);
        }

        public async Task Save<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot
        {
            await _eventStore
                .StoreEvents(aggregateRoot.Id, aggregateRoot.CurrentVersion, aggregateRoot.Events)
                .ConfigureAwait(false);

            // everything worked correctly so lets dispatch the events
            foreach (var domainEvent in aggregateRoot.Events)
            {
                await _eventDispatcher
                    .RaiseEvent(domainEvent)
                    .ConfigureAwait(false);
            }
        }

        //private static TAggregateRoot CreateInstance<TAggregateRoot>(Guid aggregateRootId)
        //    where TAggregateRoot : AggregateRoot
        //{
        //    return Activator
        //        .CreateInstance(
        //            typeof (TAggregateRoot),
        //            aggregateRootId) as TAggregateRoot;
        //}

        private TAggregateRoot GetAggregateInstance<TAggregateRoot>(
            Guid aggregateRootId,
            ICollection<DomainEvent> events)
            where TAggregateRoot : AggregateRoot
        {
            //var instance = CreateInstance<TAggregateRoot>(aggregateRootId);
            var instance = Activator
                .CreateInstance(typeof (TAggregateRoot), aggregateRootId)
                as TAggregateRoot;

            if (events != null && events.Any())
            {
                _aggregateHydrator
                    .HydrateAggregateWithEvents(instance, events);

                instance.CurrentVersion = events.Last().Metadata.StorageVersion;
            }

            return instance;
        }
    }
}