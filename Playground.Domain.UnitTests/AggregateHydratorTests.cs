using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.UnitTests
{
    [TestFixture]
    public class AggregateHydratorTests : TestBase
    {
        [Test]
        public void HydrateAggregateWithEvents_InvokesApplyForAllEvents()
        {
            // arrange
            var fakeAggregate = A.Fake<AggregateRoot>(builder => builder
                .WithArgumentsForConstructor(new List<object> {Guid.NewGuid()})
                .Implements(typeof (IEmit<Event1>))
                .Implements(typeof (IEmit<Event2>)));

            var event1 = Fixture.Create<Event1>();
            var event2 = Fixture.Create<Event2>();

            var events = new List<IEvent>
            {
                event1,
                event2
            };

            // act
            Faker
                .Resolve<AggregateHydrator>()
                .HydrateAggregateWithEvents(fakeAggregate, events);

            // assert
            A.CallTo(() => ((IEmit<Event1>) fakeAggregate)
                .Apply(event1))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => ((IEmit<Event2>) fakeAggregate)
                .Apply(event2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
