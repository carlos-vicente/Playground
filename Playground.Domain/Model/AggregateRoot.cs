using System;
using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    public abstract class AggregateRoot : Entity
    {
        internal ICollection<DomainEvent> Events { get; private set; }

        public long CurrentVersion { get; set; }

        protected AggregateRoot(Guid id) : base(id)
        {
            Events = new List<DomainEvent>();
        }

        protected void When<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : DomainEvent
        {
            Events.Add(domainEvent);

            // TODO: should this be done here?
            ((IEmit<TDomainEvent>)this).Apply(domainEvent);
        }
    }

    public abstract class AggregateRootWithState<TAggregateState> : Entity
        where TAggregateState : new()
    {
        internal ICollection<DomainEvent> Events { get; private set; }

        public long CurrentVersion { get; set; }

        public TAggregateState State { get; set; }

        protected AggregateRootWithState(Guid id) : base(id)
        {
            Events = new List<DomainEvent>();
            State = new TAggregateState();
        }

        protected AggregateRootWithState(Guid id, TAggregateState hydratedState) 
            : base(id)
        {
            Events = new List<DomainEvent>();
            State = hydratedState;
        }

        protected void When<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : DomainEvent
        {
            Events.Add(domainEvent);

            // TODO: should this be done here?
            ((IEmit<TDomainEvent>)State).Apply(domainEvent);
        }
    }
}