namespace Playground.Domain.Events
{
    public abstract class DomainEvent
    {
        public Metadata Metadata { get; set; }

        //protected DomainEvent(Guid aggregateRootId, long version)
        //{
        //    Metadata = new Metadata(aggregateRootId, version, GetType());
        //}
    }

    public abstract class DomainEventForAggregateRootWithIdentity
    {
        public MetadataForAggregateRootWithIdentity Metadata { get; set; }
    }
}