using System.Threading.Tasks;
using Playground.Domain.Events;
using Playground.Messaging;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers
{
    internal class DummyDispatcher : IEventDispatcher
    {
        public Task RaiseEvent<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
        {
            return Task.FromResult(0);
        }
    }
}