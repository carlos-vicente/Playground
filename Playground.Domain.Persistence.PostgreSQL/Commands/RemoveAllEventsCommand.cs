using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    internal class RemoveAllEventsCommand
    {
        public Guid StreamId { get; set; }
    }
}