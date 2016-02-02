using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Playground.Data.Contracts;

namespace Playground.Data.Dapper
{
    public class Connection : IConnection
    {
        private bool _disposed;
        
        public IDbConnection InnerConnection
        {
            get; private set;
        }

        public Connection(IDbConnection connection)
        {
            InnerConnection = connection;
        }

        public Task ExecuteCommand(string sql, object parameters)
        {
            InnerConnection.Execute(sql, parameters);

            return Task.FromResult(1);
        }

        public Task<T> ExecuteQuery<T>()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                InnerConnection.Dispose();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        ~Connection()
        {
            Dispose(false);
        }
    }
}