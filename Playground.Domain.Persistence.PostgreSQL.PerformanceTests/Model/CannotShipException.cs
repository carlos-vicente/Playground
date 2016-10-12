using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    internal class CannotShipException : Exception
    {
        public CannotShipException(string message) : base(message) { }
    }
}