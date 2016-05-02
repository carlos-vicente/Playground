using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Playground.Data.Contracts;

namespace Playground.Data.Dapper
{
    /// <summary>
    /// A connection implementation using the Dapper ORM
    /// </summary>
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

        public Task ExecuteCommandAsStoredProcedure(string storedProcedure, object parameters)
        {
            InnerConnection.Execute(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            return Task.FromResult(1);
        }

        public async Task<T> ExecuteQuerySingle<T>(string sql, object parameters)
        {
            return (await InnerConnection
                .QueryAsync<T>(sql, parameters)
                .ConfigureAwait(false))
                .SingleOrDefault();
        }

        public async Task<T> ExecuteQuerySingleAsStoredProcedure<T>(
            string storedProcedure,
            object parameters)
        {
            var enumerable = await InnerConnection
                .QueryAsync<T>(
                    storedProcedure,
                    parameters,
                    null,
                    null,
                    CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return enumerable.SingleOrDefault();
        }

        public async Task<IEnumerable<T>> ExecuteQueryMultiple<T>(
            string sql,
            object parameters)
        {
            return await InnerConnection
                .QueryAsync<T>(sql, parameters)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> ExecuteQueryMultipleAsStoredProcedure<T>(
            string storedProcedure, 
            object parameters)
        {
            return await InnerConnection
                .QueryAsync<T>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
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