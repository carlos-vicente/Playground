using System;
using System.Collections.Generic;

namespace Playground.DependencyResolver.Contracts
{
    public interface IDependencyResolver
    {
        TType Resolve<TType>();
        object Resolve(Type typeToResolve);

        IEnumerable<TType> ResolveAll<TType>();
        IEnumerable<object> ResolveAll(Type typeToResolve);
    }
}
