using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Playground.DependencyResolver.Autofac;
using Playground.TicketOffice.Api.AutofacRegister;
using Playground.TicketOffice.Domain.Theater.Handlers;
using Rebus.Auditing.Messages;
using Rebus.Autofac;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Serilog;
using Rebus.Transport.InMem;
using Serilog;
using Serilog.Enrichers;

namespace Playground.TicketOffice.Api.Theater
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var config = GlobalConfiguration.Configuration;

            LoadHooks();

            var builder = new ContainerBuilder();

            builder.RegisterModule<AutofacDependencyResolverModule>();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<RebusModule>();
            builder.RegisterModule<ValidationModule>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();

            SetupLogger();
            SetupRebus(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void LoadHooks()
        {
            RegistrationHook.Load();
        }

        private static void SetupRebus(IContainer container)
        {
            const string queueName = "ticketoffice.theater.in";

            Configure
                .With(new AutofacContainerAdapter(container))
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), queueName))
                //.Routing(r => r.TypeBased().MapAssemblyOf<CreateMovieTheaterCommand>(queueName))
                .Logging(rebusConfig => rebusConfig.Serilog(Log.Logger))
                .Options(o => o.EnableMessageAuditing("ticketoffice.audit"))
                .Options(o => o.SimpleRetryStrategy(maxDeliveryAttempts: 1))
                .Start();
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.With<ThreadIdEnricher>()
                .CreateLogger();
        }
    }
}
