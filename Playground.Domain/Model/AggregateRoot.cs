using System;
using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    public abstract class AggregateRoot<TAggregateState> : Entity
        where TAggregateState : class, IAggregateState, new()
    {
        public ICollection<DomainEvent> UncommittedEvents { get; private set; }
        public TAggregateState State { get; private set; }
        public long CurrentVersion { get; private set; }

        protected AggregateRoot(Guid id)
            : this(id, null, 0L)
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

    public abstract class AggregateRootWithIdentity<TAggregateState, TIdentity> 
        : EntityWithTypedIdentity<TIdentity>
        where TAggregateState : class, IAggregateState, new()
        where TIdentity : IIdentity
    {
        public ICollection<DomainEvent> UncommittedEvents { get; private set; }
        public TAggregateState State { get; private set; }
        public long CurrentVersion { get; private set; }

        protected AggregateRootWithIdentity(TIdentity identity)
            : this(identity, null, 0L)
        {
        }

        protected AggregateRootWithIdentity(
            TIdentity identity,
            TAggregateState hydratedState,
            long currentVersion)
            : base(identity)
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