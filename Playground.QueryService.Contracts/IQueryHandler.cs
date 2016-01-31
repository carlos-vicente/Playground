namespace Playground.QueryService.Contracts
{
    public interface IQueryHandler<in TQuery, out TQueryResponse>
        where TQuery : IQuery<TQueryResponse>
        where TQueryResponse : class
    {
        TQueryResponse Handle(TQuery query);
    }
}