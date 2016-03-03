using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal class RemoveEventCommand
    {
        public Guid StreamId { get; set; }
        public long EventId { get; set; }
    }
}
