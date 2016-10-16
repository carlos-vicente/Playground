using System;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.Snapshots
{
    public interface ISnapshotStore
    {
        /// <summary>
        /// Gets the lastest available snapshot for a give aggregate
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type which was recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <returns>The state snapshot; null if none is available</returns>
        Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState>(Guid streamId)
            where TAggregateState : class, IAggregateState, new();

        Task StoreNewSnapshot<TAggregateState>(Guid streamId, Snapshot<TAggregateState> snapshot)
            where TAggregateState : class, IAggregateState, new();
    }
}