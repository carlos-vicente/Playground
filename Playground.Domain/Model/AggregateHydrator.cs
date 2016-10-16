﻿using System.Collections.Generic;
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
        /// <returns>The aggregate instance with the events applied</returns>
        public TAggregateState HydrateAggregateWithEvents<TAggregateState>(
            ICollection<DomainEvent> domainEvents,
            TAggregateState snapshot)
            where TAggregateState : class, IAggregateState, new()
        {
            var state = new TAggregateState();

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
}