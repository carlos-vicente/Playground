using System;
using System.Data;
using Playground.Data.Contracts;

namespace Playground.Data.Dapper
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;
        private readonly Func<string, IDbConnection> _connectionBuilderFunc;

        public ConnectionFactory(
            string connectionString,
            Func<string, IDbConnection> connectionBuilderFunc)
        {
            _connectionString = connectionString;
            _connectionBuilderFunc = connectionBuilderFunc;
        }

        public IConnection CreateConnection()
        {
            var realConnection = _connectionBuilderFunc(_connectionString);
            realConnection.Open();
            return new Connection(realConnection);
        }
    }
}
