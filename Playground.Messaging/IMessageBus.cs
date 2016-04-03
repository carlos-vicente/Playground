using System.Threading.Tasks;
using Playground.Messaging.Commands;

namespace Playground.Messaging
{
    public interface IMessageBus
    {
        Task SendCommand<TCommand>(TCommand command)
            where TCommand : ICommand;
    }
}