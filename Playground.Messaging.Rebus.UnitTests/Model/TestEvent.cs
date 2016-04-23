using System;
using Playground.Domain.Events;

namespace Playground.Messaging.Rebus.UnitTests.Model
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}