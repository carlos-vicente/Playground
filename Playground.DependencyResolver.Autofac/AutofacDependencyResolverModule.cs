using Autofac;
using Playground.DependencyResolver.Contracts;

namespace Playground.DependencyResolver.Autofac
{
    public class AutofacDependencyResolverModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AutofacDependencyResolver>()
                .As<IDependencyResolver>()
                .InstancePerLifetimeScope();
        }
    }
}