using System;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence
{
    /// <summary>
    /// The contract for the storage context to the aggregate data (domain events storage)
    /// </summary>
    public interface IAggregateContext
    {
        /// <summary>
        /// Creates an aggregate root of the given type with the given ID, by creating its stream. Throws if an instance already exists with the given ID
        /// <param name="aggregateRootId">The aggregate root identifier to create</param>
        /// <exception cref="InvalidOperationException">Thrown if a stream with that identifier already exists</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        /// <returns>An empty aggregate</returns>
        Task<TAggregateRoot> Create<TAggregateRoot, TAggregateState>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Attempts to load the given aggregate root
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        /// <returns>Returns the aggregate instance with it's entire stream of events applied if the stream exists. An empty aggregate if the stream exists, but has no events. Null if the stream does not exists.</returns>
        Task<TAggregateRoot> TryLoad<TAggregateRoot, TAggregateState>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Attempts to load the given aggregate root
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// <exception cref="InvalidOperationException">Thrown if the stream does not exist</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new();

        /// <summary>
        /// Saves the given aggregate
        /// </summary>
        /// <typeparam name="TAggregateRoot">The aggregate root type</typeparam>
        /// <typeparam name="TAggregateState"></typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregateRoot"/> is null</exception>
        Task Save<TAggregateRoot, TAggregateState>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new();
    }
}
