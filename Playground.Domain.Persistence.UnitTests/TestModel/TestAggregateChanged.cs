using System;
using Playground.Domain.Events;

namespace Playground.Domain.Persistence.UnitTests.TestModel
{
    public class TestAggregateChanged : DomainEvent
    {
        public string NewName { get; set; }
    }
}