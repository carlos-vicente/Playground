using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal class AddEventCommand
    {
        public Guid StreamId { get; set; }

        public long EventId { get; set; }

        public string TypeName { get; set; }

        public DateTime OccurredOn { get; set; }

        public string EventBody { get; set; }
    }
}