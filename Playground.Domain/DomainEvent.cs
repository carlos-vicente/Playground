namespace Playground.Domain
{
    public abstract class DomainEvent : IEvent
    {
        public Metadata Metadata { get; set; }
    }
}