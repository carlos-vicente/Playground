using System;

namespace Playground.Domain.Persistence.PostgreSQL.Commands
{
    public class CreateEventStreamCommand
    {
        public Guid streamId { get; set; }
    }
}