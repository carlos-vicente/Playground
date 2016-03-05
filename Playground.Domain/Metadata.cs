using System;

namespace Playground.Domain
{
    public class Metadata
    {
        public Guid AggregateRootId { get; private set; }

        public DateTime OccorredOn { get; set; }

        public Metadata(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
            OccorredOn = DateTime.UtcNow;
        }
    }
}