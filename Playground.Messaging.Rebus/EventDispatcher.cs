using System.Threading.Tasks;
using Playground.Domain.Events;
using Rebus.Bus;

namespace Playground.Messaging.Rebus
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IBus _bus;

        public EventDispatcher(IBus bus)
        {
            _bus = bus;
        }

        public async Task RaiseEvent<TEvent>(TEvent domainEvent) 
            where TEvent : DomainEvent
        {
            await _bus
                .Publish(domainEvent)
                .ConfigureAwait(false);
        }
    }
}