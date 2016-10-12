using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events
{
    public class StartedFulfilment : DomainEvent
    {
        public StartedFulfilment(Guid aggregateRootId) : base(aggregateRootId)
        {
        }
    }
}