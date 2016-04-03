using System.Threading.Tasks;
using Playground.Domain.Events;

namespace Playground.Messaging
{
    public interface IEventDispatcher
    {
        Task RaiseEvent(IEvent domainEvent);
    }
}