using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal class AddEventsCommand
    {
        internal class Event
        {
            public long EventId { get; set; }

            public string TypeName { get; set; }

            public DateTime OccurredOn { get; set; }

            public string EventBody { get; set; }

            public override string ToString()
            {
                return $"'({EventId}, '{TypeName}', '{OccurredOn}', '{EventBody}')'";
            }
        }

        public Guid streamId { get; set; }

        public Event[] events { get; set; }
    }
}