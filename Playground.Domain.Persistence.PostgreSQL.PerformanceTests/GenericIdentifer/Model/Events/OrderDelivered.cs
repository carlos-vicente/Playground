using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events
{
    public class OrderDelivered : DomainEventForAggregateRootWithIdentity
    {
        public string PersonWhoReceived { get; set; }

        public OrderDelivered(string personWhoReceived)
        {
            PersonWhoReceived = personWhoReceived;
        }
    }
}