using System;

namespace Playground.Domain.Events
{
    public abstract class DomainEvent
    {
        public Metadata Metadata { get; private set; }

        protected DomainEvent(Guid aggregateRootId)
        {
            Metadata = new Metadata(aggregateRootId, GetType());
        }
    }
}