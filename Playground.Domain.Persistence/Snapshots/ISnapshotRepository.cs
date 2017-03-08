using System;
using System.Threading.Tasks;

namespace Playground.Domain.Persistence.Snapshots
{
    public interface ISnapshotRepository
    {
        Task<StoredSnapshot> GetLatestSnapshot(Guid streamId);
        Task<long?> GetLatestSnapshotVersion(Guid streamId);
        Task<bool> SaveSnapshot(Guid streamId, StoredSnapshot storedSnapshot);
    }
}