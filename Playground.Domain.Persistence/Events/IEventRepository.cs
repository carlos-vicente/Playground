using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Domain.Persistence.Events
{
    /// <summary>
    /// The contract for an event repository, in any infrastructure.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Gets all the events for the specified stream identifier
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns>Complete list of stored events</returns>
        Task<IEnumerable<StoredEvent>> GetAll(Guid streamId);

        /// <summary>
        /// Gets the specified event in a specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="eventId">Event identifier</param>
        /// <returns>The store event</returns>
        Task<StoredEvent> Get(Guid streamId, long eventId);

        /// <summary>
        /// Gets the last event in a specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns>The store event</returns>
        Task<StoredEvent> GetLastEvent(Guid streamId);

        /// <summary>
        /// Creates an event stream to store events
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns></returns>
        Task Create(Guid streamId);

        /// <summary>
        /// Adds a new event to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="storedEvent">The event to add</param>
        Task Add(Guid streamId, StoredEvent storedEvent);

        /// <summary>
        /// Adds new events to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="events">The events to add</param>
        Task Add(Guid streamId, ICollection<StoredEvent> events);

        /// <summary>
        /// Removes an event from the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="eventId">Event identifier</param>
        Task Remove(Guid streamId, long eventId);

        /// <summary>
        /// Removes all events from the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        Task Remove(Guid streamId);
    }
}