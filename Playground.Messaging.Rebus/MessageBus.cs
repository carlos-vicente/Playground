using System;
using System.Threading.Tasks;
using Playground.Messaging.Commands;

namespace Playground.Messaging.Rebus
{
    public class MessageBus : IMessageBus
    {
        public Task SendCommand<TCommand>(TCommand command) 
            where TCommand : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
