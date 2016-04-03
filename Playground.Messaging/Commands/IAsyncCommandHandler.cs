using System.Threading.Tasks;

namespace Playground.Messaging.Commands
{
    public interface IAsyncCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}