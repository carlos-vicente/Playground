using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events
{
    public class OrderCreated : DomainEventForAggregateRootWithIdentity
    {
        public string UserOrdering { get; set; }

        public string Address { get; set; }

        public string ProductId { get; set; }

        public OrderCreated(
            string userOrdering,
            string address,
            string productId)
        {
            UserOrdering = userOrdering;
            Address = address;
            ProductId = productId;
        }
    }
}