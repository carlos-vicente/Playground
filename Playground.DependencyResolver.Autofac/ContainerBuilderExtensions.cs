using System;
using System.Linq;
using Autofac;
using Playground.Core;

namespace Playground.DependencyResolver.Autofac
{
    public static class ContainerBuilderExtensions
    {
        private static readonly EqualityComparer<Type> TypeComparer = new EqualityComparer<Type>(
            (t1, t2) => t1.FullName.Equals(t2.FullName));

        public static void RegisterGenerics(this ContainerBuilder builder, Type openGenericType)
        {
            var types = AppDomain
                .CurrentDomain.GetAssemblies().ToArray()
                .SelectMany(a => a.GetTypes())
                .Where(type => type
                    .GetInterfaces()
                    .Any(interfaceType => interfaceType.IsClosedTypeOf(openGenericType)))
                .Distinct(TypeComparer).ToList();

            foreach (var type in types)
            {
                builder.RegisterType(type)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }
    }
}