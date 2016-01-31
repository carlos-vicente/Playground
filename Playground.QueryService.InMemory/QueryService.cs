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

        public T Query<T, TQuery>(TQuery query)
            where T : class
            where TQuery : IQuery<T>
        {
            var handler = _dependencyResolver.Resolve<IQueryHandler<TQuery, T>>();
            return handler.Handle(query);
        }

        public async Task<T> QueryAsync<T, TQuery>(TQuery query)
            where T : class
            where TQuery : IQuery<T>
        {
            var handler = _dependencyResolver.Resolve<IAsyncQueryHandler<TQuery, T>>();
            return await handler
                .Handle(query)
                .ConfigureAwait(false);
        }
    }
}
