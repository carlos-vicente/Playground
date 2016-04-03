using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.UnitTests.TestModel;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.UnitTests
{
    public class EventStoreTests : TestBase
    {
        private EventStore _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<EventStore>();
        }

        [Test]
        public async Task CreateEventStream_WillCreateStream()
        {
            // arrange
            var streamId = Guid.NewGuid();

            // act
            await _sut
                .CreateEventStream(streamId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .Create(streamId))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task StoreEvents_WillStoreMappedEvents_WhenStreamExistsAndIsNotBroken()
        {
            // arrange
            var streamId = Guid.NewGuid();
            var eventMetadata = new Metadata(streamId, typeof (TestAggregateChanged));
            var currentStreamVersion = Fixture.Create<long>();

            var event1 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, eventMetadata)
                .Create();
            var event2 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, eventMetadata)
                .Create();

            var lastStreamEvent = new StoredEvent(
                typeof (TestAggregateCreated).AssemblyQualifiedName,
                Fixture.Create<DateTime>(),
                Fixture.Create<string>(),
                currentStreamVersion);

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetLastEvent(streamId))
                .Returns(Task.FromResult(lastStreamEvent));

            var event1Serialiazed = Fixture.Create<string>();
            var event2Serialiazed = Fixture.Create<string>();

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Serialize(event1))
                .Returns(event1Serialiazed);

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Serialize(event2))
                .Returns(event2Serialiazed);

            var expectedEventName = typeof (TestAggregateChanged).AssemblyQualifiedName;

            var expectedEvents = new List<StoredEvent>
            {
                new StoredEvent(expectedEventName, event1.Metadata.OccorredOn, event1Serialiazed, currentStreamVersion+1),
                new StoredEvent(expectedEventName, event2.Metadata.OccorredOn, event2Serialiazed, currentStreamVersion+2)
            };

            // act
            await _sut
                .StoreEvents(streamId, currentStreamVersion, new IEvent[] { event1, event2 })
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .Add(
                    streamId,
                    A<ICollection<StoredEvent>>.That.Matches(evts => expectedEvents.All(evts.Contains))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
