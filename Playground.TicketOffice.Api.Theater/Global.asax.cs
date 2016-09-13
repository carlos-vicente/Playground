using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Playground.DependencyResolver.Autofac;
using Playground.QueryService.InMemory.Autofac;
using Playground.TicketOffice.Api.AutofacRegister;
using Playground.TicketOffice.Theater.Write.Messages;
using Rebus.Auditing.Messages;
using Rebus.Autofac;
using Rebus.Config;
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
            builder.RegisterModule<ReadModelConnectionModule>();
            builder.RegisterModule<QueryServiceModule>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();

            SetupLogger();
            SetupRebus(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void LoadHooks()
        {
            TicketOffice.Theater.Write.Handlers.RegistrationHook.Load();
            TicketOffice.Theater.Read.Handlers.RegistrationHook.Load();
        }

        private static void SetupRebus(IContainer container)
        {
            const string queueName = "ticketoffice.theater";
            var inputQueueName = string.Format("{0}.in", queueName);
            var auditQueueName = string.Format("{0}.audit", queueName);

            Configure
                .With(new AutofacContainerAdapter(container))
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), inputQueueName))
                .Routing(r => r.TypeBased().MapAssemblyOf<CreateNewMovieTheaterCommand>(inputQueueName))
                .Logging(rebusConfig => rebusConfig.Serilog(Log.Logger))
                .Options(o => o.EnableMessageAuditing(auditQueueName))
                //.Options(o => o.SimpleRetryStrategy(maxDeliveryAttempts: 1))
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
