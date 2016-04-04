using System.Threading.Tasks;
using Playground.Domain.Events;

namespace Playground.Messaging.Rebus
{
    public class EventDispatcher : IEventDispatcher
    {
        public Task RaiseEvent(IEvent domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}