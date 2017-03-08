using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events
{
    public class OrderShippingAddressChanged : DomainEventForAggregateRootWithIdentity
    {
        public string NewAddress { get; set; }

        public OrderShippingAddressChanged(string newAddress)
        {
            NewAddress = newAddress;
        }
    }
}