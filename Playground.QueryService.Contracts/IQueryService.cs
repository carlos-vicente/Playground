using System.Threading.Tasks;

namespace Playground.QueryService.Contracts
{
    public interface IQueryService
    {
        TQueryResult Query<TQuery, TQueryResult>(TQuery query)
            where TQuery : IQuery<TQueryResult>
            where TQueryResult : class;

        Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery query)
            where TQuery : IQuery<TQueryResult>
            where TQueryResult : class;
    }
}