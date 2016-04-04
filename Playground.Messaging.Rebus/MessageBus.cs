using System.Threading.Tasks;
using Playground.Messaging.Commands;
using Rebus.Bus;

namespace Playground.Messaging.Rebus
{
    public class MessageBus : IMessageBus
    {
        private readonly IBus _rebus;

        public MessageBus(IBus rebus)
        {
            _rebus = rebus;
        }

        public async Task SendCommand<TCommand>(TCommand command) 
            where TCommand : ICommand
        {
            await _rebus
                .Send(command)
                .ConfigureAwait(false);
        }
    }
}
