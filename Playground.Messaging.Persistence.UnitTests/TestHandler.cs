using System;
using System.Threading.Tasks;
using Playground.Domain.Persistence;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class TestHandler : AsyncDomainCommandHandler<Command, Aggregate>
    {
        public TestHandler(IAggregateContext aggregateContext)
            : base(aggregateContext)
        {
        }

        protected override Task HandleOnAggregate(Command command, Aggregate aggregate)
        {
            throw new NotImplementedException();
        }
    }
}