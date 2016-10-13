using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events
{
    public class OrderShippingAddressChanged : DomainEvent
    {
        public string NewAddress { get; set; }

        public OrderShippingAddressChanged(Guid aggregateRootId, string newAddress) 
            : base(aggregateRootId)
        {
            NewAddress = newAddress;
        }
    }
}