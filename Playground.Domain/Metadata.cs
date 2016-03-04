using System;

namespace Playground.Domain
{
    public class Metadata
    {
        public Guid AggregateRootId { get; private set; }

        public Metadata(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}