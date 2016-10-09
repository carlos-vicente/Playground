using Autofac;
using Playground.TicketOffice.Web.Configuration;

namespace Playground.TicketOffice.Web.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AppSettingsEndpointsConfiguration>()
                .As<IEndpointsConfiguration>()
                .SingleInstance();
        }
    }
}