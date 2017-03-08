using System;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public interface IMetricsCounter
    {
        TimeSpan ElapsedTime { get; }
    }
}