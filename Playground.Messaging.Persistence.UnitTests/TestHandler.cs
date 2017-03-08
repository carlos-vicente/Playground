using System.Threading.Tasks;
using Playground.Domain.Persistence;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class TestHandler : AsyncDomainCommandHandler<Command, Aggregate, AggregateState>
    {
        public Aggregate CalledWithAggregate { get; set; }
        public Command CalledWithCommand { get; set; }

        public TestHandler(IAggregateContext aggregateContext)
            : base(aggregateContext)
        {
        }

        protected override Task HandleOnAggregate(Command command, Aggregate aggregate)
        {
            CalledWithCommand = command;
            CalledWithAggregate = aggregate;

            return Task.FromResult(0);
        }
    }
}