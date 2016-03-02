using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Domain.Persistence
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
        Task<IEnumerable<Event>> GetAll(Guid streamId);

        /// <summary>
        /// Gets the specified event in a specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="eventId">Event identifier</param>
        /// <returns>The store event</returns>
        Task<Event> Get(Guid streamId, long eventId);

        /// <summary>
        /// Adds a new event to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="event">The event to add</param>
        Task Add(Guid streamId, Event @event);

        /// <summary>
        /// Adds new events to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="events">The events to add</param>
        Task Add(Guid streamId, IEnumerable<Event> events);

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