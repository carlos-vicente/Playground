using System;
using System.Threading.Tasks;

namespace Playground.Domain
{
    public interface IAggregateContext
    {
        /// <summary>
        /// Creates an aggregate root of the given type with the given ID. Throws if an instance already exists with the given ID
        /// <param name="aggregateRootId">The aggregate root identifier to create</param>
        /// </summary>
        Task<TAggregateRoot> Create<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Attempts to load the given aggregate root, returning null if it does not exist
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// </summary>
        Task<TAggregateRoot> TryLoad<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Attempts to load the given aggregate root, throwing an exception if it does not exist
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// </summary>
        Task<TAggregateRoot> Load<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Saves the given aggregate
        /// </summary>
        /// <typeparam name="TAggregateRoot">The aggregate root type</typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        Task Save<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot;
    }
}
