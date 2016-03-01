using System;

namespace Playground.Domain
{
    public class Event
    {
        public string TypeName { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public string EventBody { get; private set; }
        public long EventId { get; private set; }

        public Event(
            string typeName,
            DateTime occurredOn,
            string eventBody,
            long eventId = -1L)
        {
            TypeName = typeName;
            OccurredOn = occurredOn;
            EventBody = eventBody;
            EventId = eventId;
        }
        
        public IEvent ToDomainEvent()
        {
            return ToDomainEvent<IEvent>();
        }

        public TEvent ToDomainEvent<TEvent>()
            where TEvent : IEvent
        {
            var eventType = default(Type);
            try
            {
                eventType = Type.GetType(TypeName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format("Class load error, because: {0}", ex));
            }
            return default(TEvent); //TODO: what to actually do here?!?!?! -> change this to same place where this instance is built
        }

        public bool Equals(Event other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(null, other))
                return false;

            return EventId.Equals(other.EventId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Event);
        }

        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }
    }
}
