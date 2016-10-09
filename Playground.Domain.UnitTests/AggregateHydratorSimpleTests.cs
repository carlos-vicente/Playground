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
    public class AggregateHydratorSimpleTests : SimpleTestBase
    {
        [Test]
        public void HydrateAggregateWithEvents_InvokesApplyForAllEvents()
        {
            // arrange
            var fakeAggregate = A.Fake<AggregateRoot>(builder => builder
                .WithArgumentsForConstructor(new List<object> {Guid.NewGuid()})
                .Implements(typeof (IGetAppliedWith<ItHappened>))
                .Implements(typeof (IGetAppliedWith<GotDone>)));

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
            A.CallTo(() => ((IGetAppliedWith<ItHappened>) fakeAggregate)
                .Apply(event1))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => ((IGetAppliedWith<GotDone>) fakeAggregate)
                .Apply(event2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
