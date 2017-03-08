using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public class StoringHundredsOfEventsOnSaveTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<StoringHundredsOfEventsOnSaveTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      StoringHundredsOfEventsOnSaveTest       ###########");

            // arrange
            var orderAggregate = await AggregateContext
                .Create<Order, OrderState>(Guid.NewGuid())
                .ConfigureAwait(false);

            _logger.Debug("CreateOrder");
            orderAggregate.CreateOrder(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>());
            
            _logger.Debug("ChangeAddress");
            for (var i = 0 ; i < 1000; ++i)
            {
                orderAggregate.ChangeAddress(Fixture.Create<string>());
            }
            
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
                .Save<Order, OrderState>(orderAggregate)
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