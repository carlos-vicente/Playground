using System.Threading.Tasks;
using Playground.Domain.Events;

namespace Playground.Messaging
{
    public interface IEventDispatcher
    {
        Task RaiseEvent<TEvent>(TEvent domainEvent)
            where TEvent : DomainEvent;
    }

    public interface IEventDispatcherWithGenericIdentity
    {
        Task RaiseEvent<TEvent>(TEvent domainEvent)
            where TEvent : DomainEventForAggregateRootWithIdentity;
    }
}