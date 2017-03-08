using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.UnitTests.Events;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.UnitTests
{
    [TestFixture]
    public class AggregateHydratorTests : SimpleTestBase
    {
        [Test]
        public void HydrateAggregateWithEvents_InvokesApplyForAllEvents()
        {
            // arrange
            var event1 = Fixture.Create<ItHappened>();
            var event2 = Fixture.Create<GotDone>();

            var events = new List<DomainEvent>
            {
                event1,
                event2
            };

            // act
            var fakeState = Faker
                .Resolve<AggregateHydrator>()
                .HydrateAggregateWithEvents<AggregateState>(events, null);

            // assert
            fakeState
                .SomethingNotSetItHappenedNorGotDone
                .Should()
                .BeNullOrWhiteSpace();

            fakeState
                .AppliedItHappened
                .Should()
                .BeTrue();

            fakeState
                .AppliedGotDone
                .Should()
                .BeTrue();
        }

        [Test]
        public void HydrateAggregateWithEvents_WillUseSnapshotDataAsBaseLine_WhenThereIsASnapshot()
        {
            // arrange
            var event1 = Fixture.Create<ItHappened>();
            var event2 = Fixture.Create<GotDone>();

            var events = new List<DomainEvent>
            {
                event1,
                event2
            };

            var snapshotData = new AggregateState
            {
                SomethingNotSetItHappenedNorGotDone = Fixture.Create<string>()
            };

            // act
            var fakeState = Faker
                .Resolve<AggregateHydrator>()
                .HydrateAggregateWithEvents<AggregateState>(events, snapshotData);

            // assert
            fakeState
                .SomethingNotSetItHappenedNorGotDone
                .ShouldBeEquivalentTo(snapshotData.SomethingNotSetItHappenedNorGotDone);

            fakeState
                .AppliedItHappened
                .Should()
                .BeTrue();

            fakeState
                .AppliedGotDone
                .Should()
                .BeTrue();
        }
    }
}
