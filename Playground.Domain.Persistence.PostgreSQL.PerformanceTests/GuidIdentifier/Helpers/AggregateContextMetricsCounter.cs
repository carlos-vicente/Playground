using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Helpers
{
    internal class AggregateContextMetricsCounter
        : IAggregateContext, IMetricsCounter
    {
        private readonly IAggregateContext _actualAggregateContext;
        private readonly Stopwatch _counter;

        public AggregateContextMetricsCounter(IAggregateContext actualAggregateContext)
        {
            _actualAggregateContext = actualAggregateContext;
            _counter = new Stopwatch();
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot, TAggregateState>(
            Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .Create<TAggregateRoot, TAggregateState>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot, TAggregateState>(
            Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .TryLoad<TAggregateRoot, TAggregateState>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState>(
            Guid aggregateRootId)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .Load<TAggregateRoot, TAggregateState>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task Save<TAggregateRoot, TAggregateState>(
            TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : class, IAggregateState, new()
        {
            _counter.Reset();
            _counter.Start();
            await _actualAggregateContext
                .Save<TAggregateRoot, TAggregateState>(aggregateRoot)
                .ConfigureAwait(false);
            _counter.Stop();
        }

        public TimeSpan ElapsedTime => _counter.Elapsed;
    }
}