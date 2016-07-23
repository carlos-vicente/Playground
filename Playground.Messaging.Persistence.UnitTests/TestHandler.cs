using System;
using System.Threading.Tasks;
using Playground.Domain.Persistence;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class TestHandler : AsyncDomainCommandHandler<Command, Aggregate>
    {
        public Aggregate CalledWithAggregate { get; set; }
        public Command CalledWithCommand { get; set; }

        public TestHandler(IAggregateContext aggregateContext)
            : base(aggregateContext)
        {
        }

        protected override async Task HandleOnAggregate(Command command, Aggregate aggregate)
        {
            CalledWithCommand = command;
            CalledWithAggregate = aggregate;
        }
    }
}