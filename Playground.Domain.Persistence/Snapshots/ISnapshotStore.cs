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

        /// <summary>
        /// Gets the closest snaptshot to the specified version (always with a lower version)
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type which was recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <param name="version">The maximum version number to get the closest snapshot</param>
        /// <returns>The state snapshot; null if none is available</returns>
        Task<Snapshot<TAggregateState>> GetClosestSnapshot<TAggregateState>(
            Guid streamId,
            long version)
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Stores a new snapshot for this state, overriding an existing one
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type to be recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <param name="snapshot">The state snapshot to be recorded</param>
        Task StoreNewSnapshot<TAggregateState>(Guid streamId, Snapshot<TAggregateState> snapshot)
            where TAggregateState : class, IAggregateState, new();
    }

    public interface ISnapshotStoreWithGenericIdentity
    {
        /// <summary>
        /// Gets the lastest available snapshot for a give aggregate
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type which was recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <returns>The state snapshot; null if none is available</returns>
        Task<Snapshot<TAggregateState>> GetLastestSnaptshot<TAggregateState>(string streamId)
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Gets the closest snaptshot to the specified version (always with a lower version)
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type which was recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <param name="version">The maximum version number to get the closest snapshot</param>
        /// <returns>The state snapshot; null if none is available</returns>
        Task<Snapshot<TAggregateState>> GetClosestSnapshot<TAggregateState>(
            string streamId,
            long version)
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Stores a new snapshot for this state, overriding an existing one
        /// </summary>
        /// <typeparam name="TAggregateState">The aggregate state type to be recorded</typeparam>
        /// <param name="streamId">The aggregate identifier</param>
        /// <param name="snapshot">The state snapshot to be recorded</param>
        Task StoreNewSnapshot<TAggregateState>(string streamId, Snapshot<TAggregateState> snapshot)
            where TAggregateState : class, IAggregateState, new();
    }
}