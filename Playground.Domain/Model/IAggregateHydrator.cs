using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    /// <summary>
    /// The AggregateHydrator applies a stream of events onto a given aggregate,
    /// so the aggregate is on it's desired state
    /// </summary>
    public interface IAggregateHydrator
    {
        /// <summary>
        /// Apply all the events in <paramref name="domainEvents"/> to <paramref name="aggregateRootBase"/>
        /// </summary>
        /// <typeparam name="TAggregateRoot">The aggregate type</typeparam>
        /// <param name="aggregateRootBase">The aggregate to apply the domain events (it should be a clean instance)</param>
        /// <param name="domainEvents">The list of domain events to apply on to the aggregate</param>
        /// <returns>The aggregate instance with the events applied</returns>
        TAggregateRoot HydrateAggregateWithEvents<TAggregateRoot>(
            TAggregateRoot aggregateRootBase,
            ICollection<DomainEvent> domainEvents)
            where TAggregateRoot : AggregateRoot;
    }
}