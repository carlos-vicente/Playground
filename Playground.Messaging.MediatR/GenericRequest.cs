using MediatR;
using Playground.Messaging.Commands;

namespace Playground.Messaging.MediatR
{
    public class GenericRequest<TCommand> : IRequest
        where TCommand : ICommand
    {
        public GenericRequest(TCommand command)
        {
            Command = command;
        }

        public TCommand Command { get; private set; }
    }
}
