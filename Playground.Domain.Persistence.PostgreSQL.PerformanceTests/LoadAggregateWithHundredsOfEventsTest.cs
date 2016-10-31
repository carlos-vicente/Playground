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
                Fixture.Create<Guid>(),
                OrderStatus.Delivered,
                Fixture.Create<string>());

            var events = new List<DomainEvent>
            {
                new OrderCreated(id, expectedState.UserOrdering, Fixture.Create<string>(), expectedState.ProductIdToSend),
            };

            for (var i = 0; i < 999; ++i)
            {
                events.Add(new OrderShippingAddressChanged(id, Fixture.Create<string>()));
            }

            events.Add(new OrderShippingAddressChanged(id, expectedState.ShippingAddress));
            events.Add(new StartedFulfilment(id));
            events.Add(new ShipOrder(id));
            events.Add(new OrderDelivered(id, expectedState.PersonWhoReceivedOrder));

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