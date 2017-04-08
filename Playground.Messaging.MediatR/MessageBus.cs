using System;
using System.Threading.Tasks;
using Playground.Messaging.Commands;
using MediatR;

namespace Playground.Messaging.MediatR
{
    public class MessageBus : IMessageBus
    {
        private readonly IMediator _mediatr;

        public MessageBus(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        public Task SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var request = new GenericRequest<TCommand>(command);
            return _mediatr.Send(request);
        }
    }
}
