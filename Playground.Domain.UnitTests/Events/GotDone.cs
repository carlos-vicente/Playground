using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests.Events
{
    public class GotDone : DomainEvent
    {
        public GotDone(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}