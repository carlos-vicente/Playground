using System;
using System.Threading.Tasks;

namespace Playground.Data.Contracts
{
    public interface IConnection: IDisposable
    {
        Task ExecuteCommand(string sql, object parameters);

        Task<T> ExecuteQuery<T>();
    }
}