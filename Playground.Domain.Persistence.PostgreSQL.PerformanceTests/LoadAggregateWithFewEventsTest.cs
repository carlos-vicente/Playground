using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public class LoadAggregateWithFewEventsTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<LoadAggregateWithFewEventsTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      LoadAggregateWithFewEventsTest       ###########");

            // arrange
            var id = Guid.NewGuid();

            await DatabaseHelper
                .CreateEventStream(id, typeof(Order).AssemblyQualifiedName)
                .ConfigureAwait(false);

            var expectedState = new OrderState(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<Guid>(),
                OrderStatus.Delivered,
                Fixture.Create<string>());

            var events = new List<DomainEvent>
            {
                new OrderCreated(id, expectedState.UserOrdering, expectedState.ShippingAddress, expectedState.ProductIdToSend),
                new StartedFulfilment(id),
                new ShipOrder(id),
                new OrderDelivered(id, expectedState.PersonWhoReceivedOrder)
            };

            await DatabaseHelper
                .CreateEvents(id, GetStoredEvents(events))
                .ConfigureAwait(false);

            var stopWatch = new Stopwatch();

            // act
            stopWatch.Start();

            var aggregate = await AggregateContext
                .Load<Order, OrderState>(id)
                .ConfigureAwait(false);

            stopWatch.Stop();

            // assert
            Console.WriteLine(stopWatch.Elapsed.ToString());

            aggregate
                .State
                .ShouldBeEquivalentTo(expectedState);
            stopWatch
                .ElapsedMilliseconds
                .Should()
                .BeLessOrEqualTo(1000);

            _logger.Debug("###################################################################");
        }
    }
}