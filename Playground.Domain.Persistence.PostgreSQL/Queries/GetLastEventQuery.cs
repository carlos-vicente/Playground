using System;

namespace Playground.Domain.Persistence.PostgreSQL.Queries
{
    internal class GetLastEventQuery
    {
        public Guid streamId { get; set; }
    }
}