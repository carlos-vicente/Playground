using System;

namespace Playground.Domain.Persistence.PostgreSQL
{
    public class Event
    {
        public long eventid { get; set; }

        public string typename { get; set; }

        public DateTime occurredon { get; set; }

        public string eventbody { get; set; }
    }
}