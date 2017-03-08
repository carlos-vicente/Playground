using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public abstract class BaseRepository
    {
        private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

        protected BaseRepository(NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }

        protected NpgsqlCommand CreateStoredProcedureCommand(
            NpgsqlConnection connection,
            string storedProcedure)
        {
            return new NpgsqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        protected async Task<NpgsqlConnection> OpenConnection()
        {
            var conn = new NpgsqlConnection(_connectionStringBuilder);
            await conn
                .OpenAsync()
                .ConfigureAwait(false);
            return conn;
        }
    }
}