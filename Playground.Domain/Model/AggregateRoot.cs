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

            ((IEmit<TDomainEvent>)this).Apply(domainEvent);
        }
    }
}