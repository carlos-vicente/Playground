using Playground.Domain.Events;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model.Events
{
    public class OrderDelivered : DomainEvent
    {
        public string PersonWhoReceived { get; set; }

        public OrderDelivered(string personWhoReceived)
        {
            PersonWhoReceived = personWhoReceived;
        }
    }
}