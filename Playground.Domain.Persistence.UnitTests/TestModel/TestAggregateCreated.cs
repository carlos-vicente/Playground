using Playground.Domain.Events;

namespace Playground.Domain.Persistence.UnitTests.TestModel
{
    public class TestAggregateCreated : DomainEvent
    {
        public string Name { get; set; }
    }
}