using System;
using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    public abstract class AggregateRoot<TAggregateState> : Entity
        where TAggregateState : class, new()
    {
        internal ICollection<DomainEvent> UncommittedEvents { get; private set; }
        internal TAggregateState State { get; private set; }
        public long CurrentVersion { get; private set; }

        protected AggregateRoot(Guid id)
            : this(id, null, -1L)
        {
        }

        protected AggregateRoot(Guid id, TAggregateState hydratedState, long currentVersion)
            : base(id)
        {
            UncommittedEvents = new List<DomainEvent>();
            State = hydratedState ?? new TAggregateState();
            CurrentVersion = currentVersion;
        }

        protected void When<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : DomainEvent
        {
            UncommittedEvents.Add(domainEvent);

            ((IGetAppliedWith<TDomainEvent>)State).Apply(domainEvent);
        }
    }
}