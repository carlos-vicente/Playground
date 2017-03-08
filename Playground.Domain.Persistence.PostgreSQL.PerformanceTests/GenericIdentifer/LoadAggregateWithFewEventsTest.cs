using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer.Model.Events;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests.GenericIdentifer
{
    public class LoadAggregateWithFewEventsTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<LoadAggregateWithFewEventsTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      LoadAggregateWithFewEventsTest       ###########");

            // arrange
            var id = new OrderIdentity(Guid.NewGuid());

            await DatabaseHelper
                .CreateEventStreamGeneric(id.Id, typeof(Order).AssemblyQualifiedName)
                .ConfigureAwait(false);

            var expectedState = new OrderState(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                OrderStatus.Delivered,
                Fixture.Create<string>());

            var events = new List<DomainEventForAggregateRootWithIdentity>
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
                .CreateEventsGeneric(id.Id, GetStoredEvents(id.Id, events))
                .ConfigureAwait(false);

            // act
            var aggregate = await AggregateContext
                .Load<Order, OrderState, OrderIdentity>(id)
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