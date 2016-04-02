using System;

namespace Playground.Domain.Events
{
    /// <summary>
    /// This class represents the meta data of an event
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Aggregate root identifier to which aggregate root this event applies to
        /// </summary>
        public Guid AggregateRootId { get; private set; }
        
        /// <summary>
        /// When the event occurred (in UTC format to prevent timezone issues)
        /// </summary>
        public DateTime OccorredOn { get; private set; }

        /// <summary>
        /// The event's type, used for deserialization
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// The event's storage version, will only be available after the event has been stored
        /// </summary>
        public long StorageVersion { get; set; }

        /// <summary>
        /// Constructor only to be used for deserialization purposes
        /// </summary>
        public Metadata()
        {
            // For deserialization only
        }

        /// <summary>
        /// Constructor to build an event's metadata object
        /// </summary>
        /// <param name="aggregateRootId">The aggregates root id</param>
        /// <param name="eventType">The event type</param>
        public Metadata(Guid aggregateRootId, Type eventType)
        {
            AggregateRootId = aggregateRootId;
            EventType = eventType;
            OccorredOn = DateTime.UtcNow;
        }
    }
}