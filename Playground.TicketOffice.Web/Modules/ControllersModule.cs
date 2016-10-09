using Autofac;
using Playground.Http;
using Playground.TicketOffice.Web.Controllers;

namespace Playground.TicketOffice.Web.Modules
{
    public class ControllersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HomeController>()
                .WithParameter(
                    (info, context) => info.ParameterType.FullName == typeof(IHttpClient).FullName,
                    (info, context) => context
                        .ResolveNamed<IHttpClient>(RegistrationConstants.MovieTheaterClientName))
                .InstancePerLifetimeScope();
        }
    }
}