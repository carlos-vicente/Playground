using System;
using System.Threading.Tasks;
using Playground.Domain.Events;

namespace Playground.Messaging.MediatR
{
    public class EventDispatcher : IEventDispatcher
    {
        public Task RaiseEvent<TEvent>(TEvent domainEvent)
            where TEvent : DomainEvent
        {
            throw new NotImplementedException();
        }
    }

    public class EventDispatcherWithGenericIdentity : IEventDispatcherWithGenericIdentity
    {
        public Task RaiseEvent<TEvent>(TEvent domainEvent) 
            where TEvent : DomainEventForAggregateRootWithIdentity
        {
            throw new NotImplementedException();
        }
    }
}
