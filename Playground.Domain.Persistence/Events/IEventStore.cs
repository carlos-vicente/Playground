using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Domain.Persistence.Events
{
    /// <summary>
    /// The event store contract to be used by events management
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Creates the stream of events, without any events.
        /// If the stream already exists, it does not create another one, but it does not throw an exception
        /// </summary>
        /// <param name="streamId">The stream identifier to create</param>
        Task<bool> CreateEventStream(Guid streamId);

        /// <summary>
        /// Stores events in a given stream. If the stream does not exists then an InvalidOperationException
        /// </summary>
        /// <param name="streamId">The stream identifer for the events</param>
        /// <param name="eventsToStore">The events to store</param>
        Task StoreEvents(Guid streamId, ICollection<IEvent> eventsToStore);

        /// <summary>
        /// Loads all the events of a given stream
        /// </summary>
        /// <param name="streamId">The stream identifier</param>
        /// <returns>The complete list of domain events for the stream</returns>
        Task<ICollection<IEvent>> LoadAllEvents(Guid streamId);

        /// <summary>
        /// Loads a batch of events of a given stream
        /// </summary>
        /// <param name="streamId">The stream identifier</param>
        /// <param name="fromEventId">The first event identifier to load (inclusive)</param>
        /// <param name="toEventId">The last event identifier to load (inclusive)</param>
        /// <returns>The batch list of domain events for the stream</returns>
        Task<ICollection<IEvent>> LoadSelectedEvents(Guid streamId, long fromEventId, long toEventId);
    }
}