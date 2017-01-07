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
        /// Creates an event stream to store events
        /// </summary>
        /// <param name="streamId">The stream's identifier</param>
        /// <param name="streamName">The stream's name</param>
        Task CreateStream(Guid streamId, string streamName);

        /// <summary>
        /// Checks if a stream exists
        /// </summary>
        /// <param name="streamId">The stream identifier to search for</param>
        /// <returns>True if the stream already exists; False if the stream does not exist</returns>
        Task<bool> CheckStream(Guid streamId);

        /// <summary>
        /// Gets all the events for the specified stream identifier
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns>Complete list of stored events</returns>
        Task<IEnumerable<StoredEvent>> GetAll(Guid streamId);

        /// <summary>
        /// Gets all the events for the specified stream identifier
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="fromEventId">First event's identifier to retrieve</param>
        /// <returns>Complete list of stored events</returns>
        Task<IEnumerable<StoredEvent>> GetSelected(Guid streamId, long fromEventId);

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
        Task<StoredEvent> GetLast(Guid streamId);

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

    public interface IEventRepositoryForGenericIdentity
    {
        /// <summary>
        /// Creates an event stream to store events
        /// </summary>
        /// <param name="streamId">The stream's identifier</param>
        /// <param name="streamName">The stream's name</param>
        Task CreateStream(string streamId, string streamName);

        /// <summary>
        /// Checks if a stream exists
        /// </summary>
        /// <param name="streamId">The stream identifier to search for</param>
        /// <returns>True if the stream already exists; False if the stream does not exist</returns>
        Task<bool> CheckStream(string streamId);

        /// <summary>
        /// Gets all the events for the specified stream identifier
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns>Complete list of stored events</returns>
        Task<IEnumerable<StoredEvent>> GetAll(string streamId);

        /// <summary>
        /// Gets all the events for the specified stream identifier
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="fromEventId">First event's identifier to retrieve</param>
        /// <returns>Complete list of stored events</returns>
        Task<IEnumerable<StoredEvent>> GetSelected(string streamId, long fromEventId);

        /// <summary>
        /// Gets the specified event in a specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="eventId">Event identifier</param>
        /// <returns>The store event</returns>
        Task<StoredEvent> Get(string streamId, long eventId);

        /// <summary>
        /// Gets the last event in a specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <returns>The store event</returns>
        Task<StoredEvent> GetLast(string streamId);

        /// <summary>
        /// Adds a new event to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="storedEvent">The event to add</param>
        Task Add(string streamId, StoredEvent storedEvent);

        /// <summary>
        /// Adds new events to the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="events">The events to add</param>
        Task Add(string streamId, ICollection<StoredEvent> events);

        /// <summary>
        /// Removes an event from the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        /// <param name="eventId">Event identifier</param>
        Task Remove(string streamId, long eventId);

        /// <summary>
        /// Removes all events from the specified stream
        /// </summary>
        /// <param name="streamId">Stream identifier</param>
        Task Remove(string streamId);
    }
}