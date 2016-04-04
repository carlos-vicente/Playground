using Autofac;
using Playground.DependencyResolver.Autofac;
using Playground.Messaging.Commands;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class CommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterGenerics(typeof (IAsyncCommandHandler<>));
        }
    }
}