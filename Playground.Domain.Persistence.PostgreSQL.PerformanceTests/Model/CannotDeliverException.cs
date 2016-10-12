using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    internal class CannotDeliverException : Exception
    {
        public CannotDeliverException(string message) : base(message) { }
    }
}