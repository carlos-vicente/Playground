namespace Playground.Domain.Events
{
    public interface IGetAppliedWith<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        /// <summary>
        /// Applies changes to the aggregate root instance that happens as a consequence of this event
        /// </summary>
        void Apply(TDomainEvent e);
    }
}