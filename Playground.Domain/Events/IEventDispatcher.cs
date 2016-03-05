using System.Threading.Tasks;

namespace Playground.Domain.Events
{
    public interface IEventDispatcher
    {
        Task RaiseEvent(IEvent domainEvent);
    }
}