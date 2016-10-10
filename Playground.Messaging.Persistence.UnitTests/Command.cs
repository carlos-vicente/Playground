using System;
using Playground.Messaging.Commands;

namespace Playground.Messaging.Persistence.UnitTests
{
    internal class Command : DomainCommand<Aggregate, AggregateState>
    {
        public Command(Guid aggregateRootId) : base(aggregateRootId)
        {
        }
    }
}