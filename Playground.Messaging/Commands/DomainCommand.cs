using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Commands
{
    public abstract class DomainCommand<TAggregateRoot, TAggregateState>
        : ICommand
        where TAggregateRoot : AggregateRoot<TAggregateState> 
        where TAggregateState : class, IAggregateState, new()
    {
        public Guid AggregateRootId { get; private set; }

        protected DomainCommand(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}