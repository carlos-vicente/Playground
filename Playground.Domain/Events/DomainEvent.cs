namespace Playground.Domain.Events
{
    public abstract class DomainEvent : IEvent
    {
        public Metadata Metadata { get; set; }
    }
}