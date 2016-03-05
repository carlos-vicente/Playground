using System;
using System.Linq;
using System.Threading.Tasks;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;

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
            var created = await _eventStore
                .CreateEventStream(aggregateRootId)
                .ConfigureAwait(false);

            if (!created)
            {
                throw new InvalidOperationException(
                    $"A stream already for aggregate root id {aggregateRootId}");
            }

            return CreateInstance<TAggregateRoot>(aggregateRootId);
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            var events = await _eventStore
                .LoadAllEvents(aggregateRootId)
                .ConfigureAwait(false);

            if (events == null)
            {
                return null;
            }

            var instance = CreateInstance<TAggregateRoot>(aggregateRootId);

            if (events.Any())
            {
                _aggregateHydrator
                    .HydrateAggregateWithEvents(instance, events);
            }

            return instance;
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

            var instance = CreateInstance<TAggregateRoot>(aggregateRootId);

            if (events.Any())
            {
                _aggregateHydrator
                    .HydrateAggregateWithEvents(instance, events);
            }

            return instance;
        }

        public Task Save<TAggregateRoot>(TAggregateRoot aggregateRoot) 
            where TAggregateRoot : AggregateRoot
        {
            throw new NotImplementedException();
        }

        private static TAggregateRoot CreateInstance<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            return Activator
                .CreateInstance(
                    typeof (TAggregateRoot),
                    aggregateRootId) as TAggregateRoot;
        }
    }
}