using System;
using System.Collections.Generic;

namespace Playground.Domain
{
    public abstract class Aggregate : Entity
    {
        public ICollection<IEvent> Events { get; private set; }

        protected Aggregate(Guid id) : base(id)
        {
            Events = new List<IEvent>();
        }
    }
}