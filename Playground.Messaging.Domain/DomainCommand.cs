using System;
using Playground.Domain;

namespace Playground.Messaging.Domain
{
    public abstract class DomainCommand<TAggregateRoot>
        : ICommand
        where TAggregateRoot : AggregateRoot
    {
        public Metadata Metadata { get; private set; }

        protected DomainCommand(Guid aggregateRootId)
        {
            Metadata = new Metadata(aggregateRootId);
        }
    }
}