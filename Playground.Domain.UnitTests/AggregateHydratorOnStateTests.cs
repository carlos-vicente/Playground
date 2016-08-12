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
    public class AggregateHydratorOnStateTests : SimpleTestBase
    {
        [Test]
        [Ignore("Not yet done")]
        public void HydrateAggregateWithEvents_InvokesApplyForAllEvents()
        {
            // arrange
            var fakeAggregate = A.Fake<AggregateRoot>(builder => builder
                .WithArgumentsForConstructor(new List<object> {Guid.NewGuid()})
                .Implements(typeof (IEmit<ItHappened>))
                .Implements(typeof (IEmit<GotDone>)));

            var event1 = Fixture.Create<ItHappened>();
            var event2 = Fixture.Create<GotDone>();

            var events = new List<DomainEvent>
            {
                event1,
                event2
            };

            // act
            Faker
                .Resolve<AggregateHydrator>()
                .HydrateAggregateWithEvents(fakeAggregate, events);

            // assert
            A.CallTo(() => ((IEmit<ItHappened>) fakeAggregate)
                .Apply(event1))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => ((IEmit<GotDone>) fakeAggregate)
                .Apply(event2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
