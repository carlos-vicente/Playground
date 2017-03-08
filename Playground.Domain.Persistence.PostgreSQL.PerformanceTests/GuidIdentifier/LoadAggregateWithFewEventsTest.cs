using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GuidIdentifier
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
                Fixture.Create<string>(),
                OrderStatus.Delivered,
                Fixture.Create<string>());

            var events = new List<DomainEvent>
            {
                new OrderCreated(
                    expectedState.UserOrdering,
                    expectedState.ShippingAddress,
                    expectedState.ProductIdToSend),
                new OrderStartedBeingFulfilled(),
                new OrderShipped(),
                new OrderDelivered(expectedState.PersonWhoReceivedOrder)
            };

            await DatabaseHelper
                .CreateEvents(id, GetStoredEvents(id, events))
                .ConfigureAwait(false);

            // act
            var aggregate = await AggregateContext
                .Load<Order, OrderState>(id)
                .ConfigureAwait(false);

            // assert
            Console.WriteLine(MetricsCounter.ElapsedTime.ToString());

            aggregate
                .State
                .ShouldBeEquivalentTo(expectedState);
            MetricsCounter
                .ElapsedTime
                .TotalMilliseconds
                .Should()
                .BeLessOrEqualTo(MaximumAcceptedDuration);
        }
    }
}