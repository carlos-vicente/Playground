using System;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.Snapshots
{
    public interface ISnapshotRepository
    {
        Task<StoredSnapshot> GetLatestSnapshot(Guid streamId);
        Task<long?> GetLatestSnapshotVersion(Guid streamId);
        Task<bool> SaveSnapshot(Guid streamId, StoredSnapshot storedSnapshot);
    }

    public interface ISnapshotRepositoryWithGenericIdentity
    {
        Task<StoredSnapshot> GetLatestSnapshot(string streamId);
        Task<long?> GetLatestSnapshotVersion(string streamId);
        Task<bool> SaveSnapshot(string streamId, StoredSnapshot storedSnapshot);
    }
}