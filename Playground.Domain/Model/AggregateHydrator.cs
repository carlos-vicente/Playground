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
        /// Apply all the events in <paramref name="domainEvents"/> to a newly created instance of <typeparamref name="TAggregateState"/>
        /// </summary>
        /// <typeparam name="TAggregateState">The type to use when creating the state object</typeparam>
        /// <param name="domainEvents">The list of domain events to apply on to the aggregate</param>
        /// <param name="snapshot">The snapshot to use as the aggregate state's baseline</param>
        /// <returns>The aggregate instance with all the events applied</returns>
        public TAggregateState HydrateAggregateWithEvents<TAggregateState>(
            ICollection<DomainEvent> domainEvents,
            TAggregateState snapshot)
            where TAggregateState : class, IAggregateState, new()
        {
            var state = snapshot ?? new TAggregateState();

            foreach (var domainEvent in domainEvents)
            {
                // the dynamic cast makes sure the right method is called
                Apply(state, (dynamic)domainEvent);
            }

            return state;
        }

        private static void Apply<TAggregateState, TDomainEvent>(
            TAggregateState aggregateRootState,
            TDomainEvent domainEvent)
            where TAggregateState : class, new()
            where TDomainEvent : DomainEvent
        {
            ((IGetAppliedWith<TDomainEvent>)aggregateRootState).Apply(domainEvent);
        }
    }

    public class AggregateHydratorWithGenericIdentity : IAggregateHydratorWithGenericIdentity
    {
        private static void Apply<TAggregateState, TDomainEvent, TIdentity>(
            TAggregateState aggregateRootState,
            TDomainEvent domainEvent)
            where TAggregateState : class, new()
            where TDomainEvent : DomainEventForAggregateRootWithIdentity<TIdentity>
            where TIdentity : IIdentity
        {
            ((IGetAppliedWithForAggregateWithIdentity<TDomainEvent, TIdentity>)aggregateRootState)
                .Apply(domainEvent);
        }

        /// <summary>
        /// Apply all the events in <paramref name="domainEvents"/> to a newly created instance of <typeparamref name="TAggregateState"/>
        /// </summary>
        /// <typeparam name="TAggregateState">The type to use when creating the state object</typeparam>
        /// <typeparam name="TIdentity"></typeparam>
        /// <param name="domainEvents">The list of domain events to apply on to the aggregate</param>
        /// <param name="snapshot">The snapshot to use as the aggregate state's baseline</param>
        /// <returns>The aggregate instance with all the events applied</returns>
        public TAggregateState HydrateAggregateWithEvents<TAggregateState, TIdentity>(
            ICollection<DomainEventForAggregateRootWithIdentity<TIdentity>> domainEvents,
            TAggregateState snapshot)
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            var state = snapshot ?? new TAggregateState();

            foreach (var domainEvent in domainEvents)
            {
                // the dynamic cast makes sure the right method is called
                Apply(state, (dynamic)domainEvent);
            }

            return state;
        }
    }
}