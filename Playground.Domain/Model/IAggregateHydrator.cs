using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    public interface IAggregateHydrator
    {
        TAggregateRoot HydrateAggregateWithEvents<TAggregateRoot>(
            TAggregateRoot aggregateRootBase,
            ICollection<IEvent> domainEvents)
            where TAggregateRoot : AggregateRoot;
    }
}