using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer
{
    public class StoringFewEventsOnSaveTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<StoringFewEventsOnSaveTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      StoringFewEventsOnSaveTest       ###########");

            // arrange
            var orderAggregate = await AggregateContext
                .Create<Order, OrderState, OrderIdentity>(new OrderIdentity(Guid.NewGuid()))
                .ConfigureAwait(false);

            _logger.Debug("CreateOrder");
            orderAggregate.CreateOrder(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>());
            
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
            await AggregateContext
                .Save<Order, OrderState, OrderIdentity>(orderAggregate)
                .ConfigureAwait(false);

            // assert
            Console.WriteLine(MetricsCounter.ElapsedTime.ToString());
            MetricsCounter
                .ElapsedTime
                .TotalMilliseconds
                .Should()
                .BeLessOrEqualTo(MaximumAcceptedDuration);
        }
    }
}