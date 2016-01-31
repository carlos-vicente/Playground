namespace Playground.QueryService.Contracts
{
    public interface IQuery<out T> where T : class
    {
    }
}