﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer
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
                .Create<Order, OrderState, OrderIdentity>(new OrderIdentity(Guid.NewGuid()))
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