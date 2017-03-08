using System;
using Playground.Domain.Model;

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
        public Guid AggregateRootId { get; set; }
        
        /// <summary>
        /// When the event occurred (in UTC format to prevent timezone issues)
        /// </summary>
        public DateTime OccorredOn { get; set; }

        /// <summary>
        /// The event's type, used for deserialization
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// The event's storage version, will only be available after the event has been stored
        /// </summary>
        public long Version { get; set; }

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
        /// <param name="version">The version in which the event occurred</param>
        /// <param name="eventType">The event type</param>
        public Metadata(Guid aggregateRootId, long version, Type eventType)
        {
            AggregateRootId = aggregateRootId;
            Version = version;
            EventType = eventType;
            OccorredOn = DateTime.UtcNow;
        }
    }

    public class MetadataForAggregateRootWithIdentity<TIdentity>
        where TIdentity : IIdentity
    {
        /// <summary>
        /// Aggregate root identifier to which aggregate root this event applies to
        /// </summary>
        public TIdentity AggregateRootId { get; set; }

        /// <summary>
        /// When the event occurred (in UTC format to prevent timezone issues)
        /// </summary>
        public DateTime OccorredOn { get; set; }

        /// <summary>
        /// The event's type, used for deserialization
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// The event's storage version, will only be available after the event has been stored
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Constructor only to be used for deserialization purposes
        /// </summary>
        public MetadataForAggregateRootWithIdentity()
        {
            // For deserialization only
        }

        /// <summary>
        /// Constructor to build an event's metadata object
        /// </summary>
        /// <param name="aggregateRootId">The aggregates root identity</param>
        /// <param name="version">The version in which the event occurred</param>
        /// <param name="eventType">The event type</param>
        public MetadataForAggregateRootWithIdentity(TIdentity aggregateRootId, long version, Type eventType)
        {
            AggregateRootId = aggregateRootId;
            Version = version;
            EventType = eventType;
            OccorredOn = DateTime.UtcNow;
        }
    }
}