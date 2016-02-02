namespace Playground.Data.Contracts
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection();
    }
}
