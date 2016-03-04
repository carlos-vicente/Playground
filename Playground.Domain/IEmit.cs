﻿namespace Playground.Domain
{
    public interface IEmit<in TDomainEvent> where TDomainEvent : IEvent
    {
        /// <summary>
        /// Applies changes to the aggregate root instance that happens as a consequence of this event
        /// </summary>
        void Apply(TDomainEvent e);
    }
}