using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Playground.Domain.Persistence.Snapshots;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public class SnapshotRepository : BaseRepository, ISnapshotRepository
    {
        public SnapshotRepository(NpgsqlConnectionStringBuilder connectionStringBuilder)
            : base(connectionStringBuilder) { }

        public async Task<StoredSnapshot> GetLatestSnapshot(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection
                    .BeginTransaction(IsolationLevel.ReadCommitted);

                var snapshot = (await connection
                        .QueryAsync<StoredSnapshot>(
                            Queries.Scripts.GetLastestSnapshot,
                            new {streamid = streamId},
                            trans,
                            commandType: CommandType.StoredProcedure)
                        .ConfigureAwait(false))
                    .SingleOrDefault();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return snapshot;
            }
        }

        public async Task<long?> GetLatestSnapshotVersion(Guid streamId)
        {
            if (streamId == default(Guid))
                throw new ArgumentException("Pass in a valid Guid", nameof(streamId));

            using (var connection = await OpenConnection().ConfigureAwait(false))
            {
                var trans = connection
                    .BeginTransaction(IsolationLevel.ReadCommitted);

                var version = (await connection
                        .QueryAsync<long>(
                            Queries.Scripts.GetLastestSnapshotVersion,
                            new { streamid = streamId },
                            trans,
                            commandType: CommandType.StoredProcedure)
                        .ConfigureAwait(false))
                    .SingleOrDefault();

                await trans
                    .CommitAsync()
                    .ConfigureAwait(false);

                return version != default(long)
                    ? version
                    : null as long?;
            }
        }

        public Task<bool> SaveSnapshot(Guid streamId, StoredSnapshot storedSnapshot)
        {
            throw new NotImplementedException();
        }
    }
}