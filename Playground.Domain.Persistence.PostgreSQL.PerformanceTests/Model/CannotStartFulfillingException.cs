using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model
{
    internal class CannotStartFulfillingException : Exception
    {
        public CannotStartFulfillingException(string message) : base(message)
        {
            
        }
    }
}