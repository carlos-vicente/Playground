using System;
using Playground.Domain.Events;

namespace Playground.Domain.UnitTests
{
    public class GotDone : DomainEvent
    {
        public GotDone(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}