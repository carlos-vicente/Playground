using System.Threading.Tasks;

namespace Playground.QueryService.Contracts
{
    public interface IQueryService
    {
        T Query<T, TQuery>(TQuery query)
            where TQuery : IQuery<T>
            where T : class;

        Task<T> QueryAsync<T, TQuery>(TQuery query)
            where TQuery : IQuery<T>
            where T : class;
    }
}