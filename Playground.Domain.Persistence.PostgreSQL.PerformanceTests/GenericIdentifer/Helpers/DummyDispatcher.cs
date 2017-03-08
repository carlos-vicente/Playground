using System.Threading.Tasks;
using Playground.Domain.Events;
using Playground.Messaging;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers
{
    internal class DummyDispatcher : IEventDispatcherWithGenericIdentity
    {
        public Task RaiseEvent<TEvent>(TEvent domainEvent)
            where TEvent : DomainEventForAggregateRootWithIdentity
        {
            return Task.FromResult(0);
        }
    }
}