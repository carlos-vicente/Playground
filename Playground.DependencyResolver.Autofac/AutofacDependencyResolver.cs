using System;
using System.Collections.Generic;
using Autofac;
using Playground.DependencyResolver.Contracts;

namespace Playground.DependencyResolver.Autofac
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _scope;

        public AutofacDependencyResolver(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public TType Resolve<TType>()
        {
            return _scope.Resolve<TType>();
        }

        public object Resolve(Type typeToResolve)
        {
            return _scope.Resolve(typeToResolve);
        }

        public IEnumerable<TType> ResolveAll<TType>()
        {
            return _scope.Resolve<IEnumerable<TType>>();
        }

        public IEnumerable<object> ResolveAll(Type typeToResolve)
        {
            var genericType = typeof(IEnumerable<>).MakeGenericType(typeToResolve);
            return _scope.Resolve(genericType) as IEnumerable<object>;
        }
    }
}
