using Autofac;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL;
using Playground.Serialization.Jil;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class EventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<EventStore>()
                .As<IEventStore>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventRepository>()
                .As<IEventRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<EventSerializer>()
                .As<IEventSerializer>()
                .SingleInstance();
        }
    }
}