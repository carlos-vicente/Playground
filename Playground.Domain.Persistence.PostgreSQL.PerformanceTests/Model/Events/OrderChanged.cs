using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events
{
    internal class OrderChanged : DomainEvent
    {
        public string NewAddress { get; set; }

        public OrderChanged(Guid aggregateRootId, string newAddress) 
            : base(aggregateRootId)
        {
            NewAddress = newAddress;
        }
    }
}