using Autofac;
using Playground.Domain.Model;
using Playground.Domain.Persistence;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AggregateContext>()
                .As<IAggregateContext>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<AggregateHydrator>()
                .As<IAggregateHydrator>()
                .SingleInstance();
        }
    }
}