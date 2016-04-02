using System;
using Playground.Domain.Model;

namespace Playground.Messaging.Domain
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
}