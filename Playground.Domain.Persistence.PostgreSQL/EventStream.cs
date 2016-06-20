using System;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public class EventStream
    {
        public Guid EventStreamId { get; set; }

        public string EventStreamName { get; set; }
    }
}