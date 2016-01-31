using System.Threading.Tasks;

namespace Playground.QueryService.Contracts
{
    public interface IAsyncQueryHandler<in TQuery, TQueryResponse>
        where TQuery : IQuery<TQueryResponse>
        where TQueryResponse : class
    {
        Task<TQueryResponse> Handle(TQuery query);
    }
}