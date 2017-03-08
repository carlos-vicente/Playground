using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers
{
    public interface IMetricsCounter
    {
        TimeSpan ElapsedTime { get; }
    }
}