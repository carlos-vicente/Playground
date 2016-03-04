using System;
using System.Threading.Tasks;
using Playground.Domain.Persistence.Events;

namespace Playground.Domain.Persistence
{
    public class AggregateContext : IAggregateContext
    {
        private readonly IEventStore _eventStore;

        public AggregateContext(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            var created = await _eventStore
                .CreateEventStream(aggregateRootId)
                .ConfigureAwait(false);

            return Activator
                .CreateInstance(typeof (TAggregateRoot), aggregateRootId) as TAggregateRoot;
        }

        public Task<TAggregateRoot> TryLoad<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            throw new NotImplementedException();
        }

        public Task<TAggregateRoot> Load<TAggregateRoot>(Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot
        {
            throw new NotImplementedException();
        }

        public Task Save<TAggregateRoot>(TAggregateRoot aggregateRoot) 
            where TAggregateRoot : AggregateRoot
        {
            throw new NotImplementedException();
        }
    }
}