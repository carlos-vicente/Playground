﻿using System;

namespace Playground.Domain.Persistence.PostgreSQL.Queries
{
    internal class GetEventQuery
    {
        public Guid StreamId { get; set; }
        public long EventId { get; set; }
    }
}