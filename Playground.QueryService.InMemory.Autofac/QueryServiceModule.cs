using Autofac;
using Playground.QueryService.Contracts;
using Playground.DependencyResolver.Autofac;

namespace Playground.QueryService.InMemory.Autofac
{
    /// <summary>
    /// An autofac module to register all implementations IQueryHandler and IAsyncQueryHandler.
    /// The assemblies to consider must be already loaded into the current AppDomain.
    /// </summary>
    public class QueryServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGenerics(typeof(IQueryHandler<,>));
            builder.RegisterGenerics(typeof(IAsyncQueryHandler<,>));

            builder
                .RegisterType<QueryService>()
                .As<IQueryService>()
                .InstancePerLifetimeScope();
        }
    }
}
