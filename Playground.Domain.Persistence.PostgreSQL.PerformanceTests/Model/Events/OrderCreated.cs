using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events
{
    public class OrderCreated : DomainEvent
    {
        public string UserOrdering { get; set; }

        public string Address { get; set; }

        public Guid ProductId { get; set; }

        public OrderCreated(Guid aggregateRootId, string userOrdering, string address, Guid productId) 
            : base(aggregateRootId)
        {
            UserOrdering = userOrdering;
            Address = address;
            ProductId = productId;
        }
    }
}