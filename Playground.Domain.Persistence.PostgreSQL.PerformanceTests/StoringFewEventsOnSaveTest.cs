using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public class StoringFewEventsOnSaveTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<StoringFewEventsOnSaveTest>();

        [Test]
        public async Task Execute()
        {
            // arrange
            var orderAggregate = await AggregateContext
                .Create<Order, OrderState>(Guid.NewGuid())
                .ConfigureAwait(false);

            var stopWatch = new Stopwatch();

            _logger.Debug("CreateOrder");
            orderAggregate
                .CreateOrder(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<Guid>());


            _logger.Debug("StartFulfilling");
            orderAggregate
                .StartFulfilling();

            _logger.Debug("Ship");
            orderAggregate
                .Ship();

            _logger.Debug("Deliver");
            orderAggregate
                .Deliver(Fixture.Create<string>());
            
            // act
            stopWatch.Start();

            await AggregateContext
                .Save<Order, OrderState>(orderAggregate)
                .ConfigureAwait(false);

            stopWatch.Stop();

            // assert
            Console.WriteLine(stopWatch.Elapsed.ToString());
            stopWatch
                .ElapsedMilliseconds
                .Should()
                .BeLessOrEqualTo(1000);
        }
    }
}