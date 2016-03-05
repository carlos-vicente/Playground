using System;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence
{
    public interface IAggregateContext
    {
        /// <summary>
        /// Creates an aggregate root of the given type with the given ID. Throws if an instance already exists with the given ID
        /// <param name="aggregateRootId">The aggregate root identifier to create</param>
        /// <exception cref="InvalidOperationException">Thrown if a stream with that identifier already exists</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        Task<TAggregateRoot> Create<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Attempts to load the given aggregate root, returning null if it does not exist
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        Task<TAggregateRoot> TryLoad<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Attempts to load the given aggregate root, throwing an exception if it does not exist
        /// <param name="aggregateRootId">The aggregate root identifier to look for</param>
        /// <exception cref="InvalidOperationException">Thrown if the stream does not exist</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateRootId"/> is Guid's default value</exception>
        /// </summary>
        Task<TAggregateRoot> Load<TAggregateRoot>(Guid aggregateRootId) 
            where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// Saves the given aggregate
        /// </summary>
        /// <typeparam name="TAggregateRoot">The aggregate root type</typeparam>
        /// <param name="aggregateRoot">The aggregate root</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregateRoot"/> is null</exception>
        Task Save<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot;
    }
}
