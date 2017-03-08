using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoadAggregateWithSnaphostAndFewEventsTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<LoadAggregateWithSnaphostAndFewEventsTest>();

        [Test]
        public async Task Execute()
        {
            _logger.Debug("#############      LoadAggregateWithSnaphostAndFewEventsTest       ###########");

            // arrange
            var id = new OrderIdentity(Guid.NewGuid());
            var streamName = Fixture.Create<string>();

            await DatabaseHelper
                .CreateEventStreamGeneric(id.Id, streamName)
                .ConfigureAwait(false);

            var orderCreatedEvent = new OrderCreated(
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>());

            var addressChangedEvents = new List<OrderShippingAddressChanged>();
            for (var i = 0; i < 1000; ++i)
            {
                addressChangedEvents.Add(new OrderShippingAddressChanged(Fixture.Create<string>()));
            }

            var orderStartedBeingFulfilledEvent = new OrderStartedBeingFulfilled();
            var orderShippedEvent = new OrderShipped();
            var orderDeliveredEvent = new OrderDelivered(Fixture.Create<string>());

            var domainEvents = new List<DomainEventForAggregateRootWithIdentity>();
            domainEvents.Add(orderCreatedEvent);
            domainEvents.AddRange(addressChangedEvents);
            domainEvents.Add(orderStartedBeingFulfilledEvent);
            domainEvents.Add(orderShippedEvent);
            domainEvents.Add(orderDeliveredEvent);

            var storedEvents = GetStoredEvents(id.Id, domainEvents);

            var version = storedEvents
                .Single(ev => ev.EventType == typeof(OrderStartedBeingFulfilled))
                .EventId;

            await DatabaseHelper
                .CreateEventsGeneric(id.Id, storedEvents)
                .ConfigureAwait(false);

            var snapshotData = new OrderState(
                orderCreatedEvent.UserOrdering,
                addressChangedEvents.Last().NewAddress,
                orderCreatedEvent.ProductId,
                OrderStatus.BeingFulfilled,
                null);

            var storedSnapshot = GetStoredSnapshot<OrderState>(version, snapshotData);

            await DatabaseHelper
                .CreateSnapshotGeneric(id.Id, storedSnapshot)
                .ConfigureAwait(false);

            var expectedState = new OrderState(
                orderCreatedEvent.UserOrdering,
                addressChangedEvents.Last().NewAddress,
                orderCreatedEvent.ProductId,
                OrderStatus.Delivered,
                orderDeliveredEvent.PersonWhoReceived);

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