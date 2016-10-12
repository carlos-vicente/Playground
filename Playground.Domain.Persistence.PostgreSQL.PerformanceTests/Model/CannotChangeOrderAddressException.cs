using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    public class CannotChangeOrderAddressException : Exception
    {
        public CannotChangeOrderAddressException(string message) : base(message)
        {
        }
    }
}