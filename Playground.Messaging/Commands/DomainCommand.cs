using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Commands
{
    public abstract class DomainCommand<TAggregateRoot>
        : ICommand
        where TAggregateRoot : AggregateRoot
    {
        public Guid AggregateRootId { get; private set; }

        protected DomainCommand(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }

    public abstract class StatedDomainCommand<TAggregateRoot, TAggregateState>
        : ICommand
        where TAggregateRoot : AggregateRootWithState<TAggregateState> 
        where TAggregateState : new()
    {
        public Guid AggregateRootId { get; private set; }

        protected StatedDomainCommand(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}