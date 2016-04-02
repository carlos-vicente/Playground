using System;
using System.Threading.Tasks;
using Playground.DependencyResolver.Contracts;
using Playground.QueryService.Contracts;

namespace Playground.QueryService.InMemory
{
    public class QueryService : IQueryService
    {
        private readonly IDependencyResolver _dependencyResolver;

        public QueryService(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public TQueryResult Query<TQuery, TQueryResult>(TQuery query)
            where TQueryResult : class
            where TQuery : IQuery<TQueryResult>
        {
            var handler = _dependencyResolver
                .Resolve<IQueryHandler<TQuery, TQueryResult>>();

            if(handler == null)
                throw new InvalidOperationException($"Failed to resolve implementation of {typeof(IQueryHandler<TQuery, TQueryResult>)}");

            return handler.Handle(query);
        }

        public async Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery query)
            where TQueryResult : class
            where TQuery : IQuery<TQueryResult>
        {
            var handler = _dependencyResolver
                .Resolve<IAsyncQueryHandler<TQuery, TQueryResult>>();

            if (handler == null)
                throw new InvalidOperationException($"Failed to resolve implementation of {typeof(IQueryHandler<TQuery, TQueryResult>)}");

            return await handler
                .Handle(query)
                .ConfigureAwait(false);
        }
    }
}
