using System;

namespace Playground.Domain.Events
{
    public abstract class DomainEvent : IEvent
    {
        public Metadata Metadata { get; set; }

        protected DomainEvent(Guid aggregateRootId)
        {
            Metadata = new Metadata(aggregateRootId, GetType());
        }
    }
}