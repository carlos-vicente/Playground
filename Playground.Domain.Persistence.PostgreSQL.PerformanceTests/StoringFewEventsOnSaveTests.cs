using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    [TestFixture]
    public class StoringFewEventsOnSaveTests : AggregateContextPerformanceTestBase
    {
        [Test]
        public async Task Execute()
        {
            // arrange
            var orderAggregate = await AggregateContext
                .Create<Order, OrderState>(Guid.NewGuid())
                .ConfigureAwait(false);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            orderAggregate
                .CreateOrder(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<Guid>());

            orderAggregate
                .StartFulfilling();

            orderAggregate
                .Ship();

            orderAggregate
                .Deliver(Fixture.Create<string>());

            // act
            await AggregateContext
                .Save<Order, OrderState>(orderAggregate)
                .ConfigureAwait(false);

            stopWatch.Stop();

            // assert
            stopWatch
                .ElapsedMilliseconds
                .Should()
                .BeLessOrEqualTo(50);
        }
    }
}