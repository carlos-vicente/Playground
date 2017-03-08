using System;
using Playground.Domain.Model;

namespace Playground.Domain.Events
{
    public abstract class DomainEvent
    {
        public Metadata Metadata { get; internal set; }

        //protected DomainEvent(Guid aggregateRootId, long version)
        //{
        //    Metadata = new Metadata(aggregateRootId, version, GetType());
        //}
    }

    public abstract class DomainEventForAggregateRootWithIdentity<TIdentity>
        where TIdentity : IIdentity
    {
        public MetadataForAggregateRootWithIdentity<TIdentity> Metadata { get; internal set; }

        //protected DomainEvent(Guid aggregateRootId, long version)
        //{
        //    Metadata = new Metadata(aggregateRootId, version, GetType());
        //}
    }
}