using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    public class AddEventCommand
    {
        public Guid StreamId { get; set; }
    }
}