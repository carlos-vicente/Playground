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
    public class LoadAggregateWithHundredsOfEventsTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<LoadAggregateWithHundredsOfEventsTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      LoadAggregateWithHundredsOfEventsTest       ###########");

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
                new OrderCreated(expectedState.UserOrdering, Fixture.Create<string>(), expectedState.ProductIdToSend),
            };

            for (var i = 0; i < 999; ++i)
            {
                events.Add(new OrderShippingAddressChanged(Fixture.Create<string>()));
            }

            events.Add(new OrderShippingAddressChanged(expectedState.ShippingAddress));
            events.Add(new OrderStartedBeingFulfilled());
            events.Add(new OrderShipped());
            events.Add(new OrderDelivered(expectedState.PersonWhoReceivedOrder));

            await DatabaseHelper
                .CreateEvents(id, GetStoredEvents(events))
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