using System.Threading.Tasks;

namespace Playground.Messaging
{
    public interface IAsyncCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}