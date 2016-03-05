using System;
using System.Collections.Generic;
using Playground.Domain.Events;

namespace Playground.Domain.Model
{
    public abstract class AggregateRoot : Entity
    {
        public ICollection<DomainEvent> Events { get; private set; }

        protected AggregateRoot(Guid id) : base(id)
        {
            Events = new List<DomainEvent>();
        }
    }
}