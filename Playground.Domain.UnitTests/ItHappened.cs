using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests
{
    public class ItHappened : DomainEvent
    {
        public string Name { get; set; }

        public ItHappened(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}