using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.UnitTests.TestModel;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.UnitTests
{
    public class EventStoreTests : TestBaseWithSut<EventStore>
    {
        [Test]
        public async Task CreateEventStream_WillCreateStream()
        {
            // arrange
            var streamId = Guid.NewGuid();

            // act
            await Sut
                .CreateEventStream(streamId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .CreateStream(streamId))
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
                new StoredEvent(expectedEventName, event1.Metadata.OccorredOn, event1Serialiazed),
                new StoredEvent(expectedEventName, event2.Metadata.OccorredOn, event2Serialiazed)
            };

            // act
            await Sut
                .StoreEvents(streamId, currentStreamVersion, new IEvent[] { event1, event2 })
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .Add(
                    streamId,
                    A<ICollection<StoredEvent>>.That.Matches(evts => expectedEvents.All(evts.Contains))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task StoreEvents_WillStoreMappedEvents_WhenStreamExistsAndIsEmpty()
        {
            // arrange
            var streamId = Guid.NewGuid();
            var eventMetadata = new Metadata(streamId, typeof(TestAggregateChanged));
            
            var event1 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, eventMetadata)
                .Create();
            var event2 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, eventMetadata)
                .Create();

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetLastEvent(streamId))
                .Returns(Task.FromResult<StoredEvent>(null));

            var event1Serialiazed = Fixture.Create<string>();
            var event2Serialiazed = Fixture.Create<string>();

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Serialize(event1))
                .Returns(event1Serialiazed);

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Serialize(event2))
                .Returns(event2Serialiazed);

            var expectedEventName = typeof(TestAggregateChanged).AssemblyQualifiedName;

            var expectedEvents = new List<StoredEvent>
            {
                new StoredEvent(expectedEventName, event1.Metadata.OccorredOn, event1Serialiazed),
                new StoredEvent(expectedEventName, event2.Metadata.OccorredOn, event2Serialiazed)
            };

            // act
            await Sut
                .StoreEvents(streamId, 0L, new IEvent[] { event1, event2 })
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .Add(
                    streamId,
                    A<ICollection<StoredEvent>>.That.Matches(evts => expectedEvents.All(evts.Contains))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void StoreEvents_WillThrowException_WhenStreamExistsAndIsBroken()
        {
            // arrange
            var streamId = Guid.NewGuid();
            var eventMetadata = new Metadata(streamId, typeof(TestAggregateChanged));
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
                typeof(TestAggregateCreated).AssemblyQualifiedName,
                Fixture.Create<DateTime>(),
                Fixture.Create<string>(),
                currentStreamVersion + 1);

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetLastEvent(streamId))
                .Returns(Task.FromResult(lastStreamEvent));

            Func<Task> exceptionThrower = async () => await Sut
                .StoreEvents(streamId, currentStreamVersion, new IEvent[] {event1, event2})
                .ConfigureAwait(false);

            // act & assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>()
                .And
                .Message
                .Should()
                .Contain($"Cant add new events on version {currentStreamVersion} as current storage version is {lastStreamEvent.EventId}");

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .Add(streamId, A<ICollection<StoredEvent>>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LoadAllEvents_WillLoadAndMapAllEvents_WhenThereAreStoredEvents()
        {
            // arrange
            var streamId = Guid.NewGuid();

            var event1Metadata = new Metadata(streamId, typeof(TestAggregateChanged))
            {
                StorageVersion = 1L
            };
            var event2Metadata = new Metadata(streamId, typeof(TestAggregateChanged))
            {
                StorageVersion = 2L
            };
            var typeName = typeof (TestAggregateChanged).AssemblyQualifiedName;

            var event1 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, event1Metadata)
                .Create();
            var event2 = Fixture
                .Build<TestAggregateChanged>()
                .With(e => e.Metadata, event2Metadata)
                .Create();

            var expectedEvents = new List<IEvent>
            {
                event1,
                event2
            };

            var event1Serialized = Fixture.Create<string>();
            var event2Serialized = Fixture.Create<string>();

            var storedEvents = new List<StoredEvent>
            {
                new StoredEvent(typeName, event1Metadata.OccorredOn, event1Serialized, event1Metadata.StorageVersion),
                new StoredEvent(typeName, event2Metadata.OccorredOn, event2Serialized, event2Metadata.StorageVersion)
            };

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetAll(streamId))
                .Returns(storedEvents);

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Deserialize(event1Serialized))
                .Returns(event1);
            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Deserialize(event2Serialized))
                .Returns(event2);

            // act
            var events = await Sut
                .LoadAllEvents(streamId)
                .ConfigureAwait(false);

            // assert
            events
                .ShouldAllBeEquivalentTo(expectedEvents);
        }

        [Test]
        public async Task LoadAllEvents_WillReturnEmptyList_WhenThereAreNoStoredEvents()
        {
            // arrange
            var streamId = Guid.NewGuid();

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetAll(streamId))
                .Returns(new List<StoredEvent>());
            
            // act
            var events = await Sut
                .LoadAllEvents(streamId)
                .ConfigureAwait(false);

            // assert
            events
                .Should()
                .BeEmpty();

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Deserialize(A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LoadAllEvents_WillReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // arrange
            var streamId = Guid.NewGuid();

            A.CallTo(() => Faker.Resolve<IEventRepository>()
                .GetAll(streamId))
                .Returns(null as List<StoredEvent>);

            // act
            var events = await Sut
                .LoadAllEvents(streamId)
                .ConfigureAwait(false);

            // assert
            events
                .Should()
                .BeEmpty();

            A.CallTo(() => Faker.Resolve<IEventSerializer>()
                .Deserialize(A<string>._))
                .MustNotHaveHappened();
        }
    }
}
