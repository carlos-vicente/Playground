using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Playground.Domain.Model;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers
{
    internal class AggregateContextMetricsCounter
        : IAggregateContextForAggregateWithIdentity, IMetricsCounter
    {
        private readonly IAggregateContextForAggregateWithIdentity _actualAggregateContext;
        private readonly Stopwatch _counter;

        public AggregateContextMetricsCounter(IAggregateContextForAggregateWithIdentity actualAggregateContext)
        {
            _actualAggregateContext = actualAggregateContext;
            _counter = new Stopwatch();
        }

        public async Task<TAggregateRoot> Create<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .Create<TAggregateRoot, TAggregateState, TIdentity>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task<TAggregateRoot> TryLoad<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .TryLoad<TAggregateRoot, TAggregateState, TIdentity>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task<TAggregateRoot> Load<TAggregateRoot, TAggregateState, TIdentity>(
            TIdentity aggregateRootId)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            _counter.Reset();
            _counter.Start();
            var root = await _actualAggregateContext
                .Load<TAggregateRoot, TAggregateState, TIdentity>(aggregateRootId)
                .ConfigureAwait(false);
            _counter.Stop();
            return root;
        }

        public async Task Save<TAggregateRoot, TAggregateState, TIdentity>(
            TAggregateRoot aggregateRoot)
            where TAggregateRoot : AggregateRootWithIdentity<TAggregateState, TIdentity>
            where TAggregateState : class, IAggregateState, new()
            where TIdentity : IIdentity
        {
            _counter.Reset();
            _counter.Start();
            await _actualAggregateContext
                .Save<TAggregateRoot, TAggregateState, TIdentity>(aggregateRoot)
                .ConfigureAwait(false);
            _counter.Stop();
        }

        public TimeSpan ElapsedTime => _counter.Elapsed;
    }
}