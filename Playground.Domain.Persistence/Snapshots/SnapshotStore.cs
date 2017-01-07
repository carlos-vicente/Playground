using System;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.Snapshots
{
    public class SnapshotStore : ISnapshotStore
    {
        private readonly ISnapshotRepository _repository;

        public SnapshotStore(ISnapshotRepository repository)
        {
            _repository = repository;
        }

        public Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState>(
            Guid streamId)
            where TAggregateState : class, IAggregateState, new()
        {
            throw new NotImplementedException();
        }

        public Task StoreNewSnapshot<TAggregateState>(
            Guid streamId,
            Snapshot<TAggregateState> snapshot) 
            where TAggregateState : class, IAggregateState, new()
        {
            throw new NotImplementedException();
        }
    }

    public class SnapshotStoreWithGenericIdentity : ISnapshotStoreWithGenericIdentity
    {
        public async Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState, TIdentity>(
            TIdentity streamId)
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            throw new NotImplementedException();
        }

        public async Task StoreNewSnapshot<TAggregateState, TIdentity>(
            TIdentity streamId,
            Snapshot<TAggregateState> snapshot)
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            throw new NotImplementedException();
        }
    }
}