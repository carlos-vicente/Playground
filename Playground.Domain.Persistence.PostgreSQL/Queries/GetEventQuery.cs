using System;

namespace Playground.Domain.Persistence.PostgreSQL.Queries
{
    public class GetEventQuery
    {
        public Guid StreamId { get; set; }
        public long EventId { get; set; }
    }
}