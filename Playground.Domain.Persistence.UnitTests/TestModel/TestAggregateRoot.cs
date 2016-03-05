using System;
using Playground.Domain.Events;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.UnitTests.TestModel
{
    internal class TestAggregateRoot 
        : AggregateRoot,
        IEmit<TestAggregateCreated>,
        IEmit<TestAggregateChanged>
    {
        public TestAggregateRoot(Guid id) : base(id) { }

        void IEmit<TestAggregateCreated>.Apply(TestAggregateCreated e)
        {
            throw new NotImplementedException();
        }

        void IEmit<TestAggregateChanged>.Apply(TestAggregateChanged e)
        {
            throw new NotImplementedException();
        }
    }
}