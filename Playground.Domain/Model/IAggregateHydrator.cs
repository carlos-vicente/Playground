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
        /// Apply all the events in <paramref name="domainEvents"/> to a newly created instance of <typeparamref name="TAggregateState"/>
        /// </summary>
        /// <typeparam name="TAggregateState">The type to use when creating the state object</typeparam>
        /// <param name="domainEvents">The list of domain events to apply on to the aggregate</param>
        /// <param name="snapshot">The lastest snapshot for this aggregate state; NULL if no snapshot is available</param>
        /// <returns>The aggregate instance with the events applied</returns>
        TAggregateState HydrateAggregateWithEvents<TAggregateState>(
            ICollection<DomainEvent> domainEvents,
            TAggregateState snapshot)
            where TAggregateState : class, IAggregateState, new();
    }
}