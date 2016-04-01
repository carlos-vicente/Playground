using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    /// <summary>
    /// The AggregateHydrator applies a stream of events onto a given aggregate,
    /// so the aggregate is on it's desired state
    /// </summary>
    public class AggregateHydrator : IAggregateHydrator
    {
        /// <summary>
        /// Apply all the events in <paramref name="domainEvents"/> to <paramref name="aggregateRootBase"/>
        /// </summary>
        /// <typeparam name="TAggregateRoot">The aggregate type</typeparam>
        /// <param name="aggregateRootBase">The aggregate to apply the domain events (it should be a clean instance)</param>
        /// <param name="domainEvents">The list of domain events to apply on to the aggregate</param>
        /// <returns>The aggregate instance with the events applied</returns>
        public TAggregateRoot HydrateAggregateWithEvents<TAggregateRoot>(
            TAggregateRoot aggregateRootBase, 
            ICollection<IEvent> domainEvents) 
            where TAggregateRoot : AggregateRoot
        {
            foreach (var domainEvent in domainEvents)
            {
                // the dynamic cast makes sure the right method is called
                Apply(aggregateRootBase, (dynamic)domainEvent);
            }

            return aggregateRootBase;
        }

        private static void Apply<TAggregateRoot, TDomainEvent>(
            TAggregateRoot aggregateRootBase,
            TDomainEvent domainEvent)
            where TAggregateRoot : AggregateRoot
            where TDomainEvent : IEvent
        {
            ((IEmit<TDomainEvent>) aggregateRootBase)
                .Apply(domainEvent);
        }
    }
}