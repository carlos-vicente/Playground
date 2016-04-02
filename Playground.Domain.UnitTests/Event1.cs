using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests
{
    public class Event1 : DomainEvent
    {
        public string Name { get; set; }

        public Event1(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}