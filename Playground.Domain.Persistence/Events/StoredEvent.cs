using System;

namespace Playground.Domain.Persistence.Events
{
    public class StoredEvent
    {
        public long EventId { get; set; }

        public string TypeName { get; set; }

        public DateTime OccurredOn { get; set; }

        public Guid BatchId { get; set; }

        public string EventBody { get; set; }

        public Type EventType
        {
            get { return Type.GetType(TypeName); }
        }

        public StoredEvent()
        {
            // Used only for mapping & deserialization
        }

        public StoredEvent(
            string typeName,
            DateTime occurredOn,
            string eventBody,
            Guid batchId,
            long eventId = -1L)
        {
            TypeName = typeName;
            OccurredOn = occurredOn;
            EventBody = eventBody;
            BatchId = batchId;
            EventId = eventId;
        }

        public bool Equals(StoredEvent other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(null, other))
                return false;

            return EventId.Equals(other.EventId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StoredEvent);
        }

        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }
    }
}
