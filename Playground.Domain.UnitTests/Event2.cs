using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests
{
    public class Event2 : DomainEvent
    {
        public Event2(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}