using System;
using System.Collections.Generic;

namespace Playground.Domain
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