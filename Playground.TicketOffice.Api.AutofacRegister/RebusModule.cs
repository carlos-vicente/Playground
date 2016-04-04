using Autofac;
using Playground.Messaging;
using Playground.Messaging.Rebus;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class RebusModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MessageBus>()
                .As<IMessageBus>()
                .SingleInstance();

            builder
                .RegisterType<EventDispatcher>()
                .As<IEventDispatcher>()
                .SingleInstance();
        }
    }
}
