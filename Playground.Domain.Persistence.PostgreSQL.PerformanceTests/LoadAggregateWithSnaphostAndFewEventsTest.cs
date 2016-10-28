using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Helpers;
using Playground.Domain.Persistence.PostgreSQL.PerformanceTests.Model;
using Ploeh.AutoFixture;
using Serilog;

namespace Playground.Domain.Persistence.PostgreSQL.PerformanceTests
{
    public class LoadAggregateWithSnaphostAndFewEventsTest : AggregateContextPerformanceTestBase
    {
        private readonly ILogger _logger = Log.ForContext<LoadAggregateWithSnaphostAndFewEventsTest>();

        [Test]
        public async Task Execute()
        {
            //_logger.Debug("#############      LoadAggregateWithHundredsOfEventsTest       ###########");

            //// arrange
            //var id = Guid.NewGuid();

            //var orderAggregate = await AggregateContext
            //    .Create<Order, OrderState>(id)
            //    .ConfigureAwait(false);

            //var stopWatch = new Stopwatch();

            //orderAggregate.CreateOrder(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<Guid>());
            //for (var i = 0; i < 1000; ++i)
            //{
            //    orderAggregate.ChangeAddress(Fixture.Create<string>());
            //}
            //orderAggregate.StartFulfilling();
            //orderAggregate.Ship();
            //orderAggregate.Deliver(Fixture.Create<string>());

            //await AggregateContext
            //    .Save<Order, OrderState>(orderAggregate)
            //    .ConfigureAwait(false);

            //// act
            //stopWatch.Start();

            //var aggregate = await AggregateContext
            //    .Load<Order, OrderState>(id)
            //    .ConfigureAwait(false);

            //stopWatch.Stop();

            //// assert
            //Console.WriteLine(stopWatch.Elapsed.ToString());

            //aggregate
            //    .State
            //    .ShouldBeEquivalentTo(orderAggregate.State);
            //stopWatch
            //    .ElapsedMilliseconds
            //    .Should()
            //    .BeLessOrEqualTo(1000);

            //_logger.Debug("###################################################################");

            // TODO: implement this test
            // TODO: stop using the domain model to setup the tests, use testing infrastructure code
        }
    }
}