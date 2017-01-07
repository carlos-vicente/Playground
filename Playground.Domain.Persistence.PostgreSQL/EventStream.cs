using System;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public class EventStream
    {
        public Guid EventStreamId { get; set; }

        public string EventStreamName { get; set; }
    }

    public class EventStreamForGenericIdentity
    {
        public string EventStreamId { get; set; }

        public string EventStreamName { get; set; }
    }
}