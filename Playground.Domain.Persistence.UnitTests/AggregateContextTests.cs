using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.UnitTests.TestModel;
using Playground.Messaging;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.UnitTests
{
    public class AggregateContextTests : TestBaseWithSut<AggregateContext>
    {
        [Test]
        public async Task Create_ReturnsNewInstanceWithId_WhenStreamIsSuccessfullyCreated()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .CreateEventStream<TestAggregateRoot>(aggregateRootId))
                .Returns(Task.FromResult(true));

            // act
            var aggregateRoot = await Sut
                .Create<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(true);

            // assert
            aggregateRoot.Should().NotBeNull();
            aggregateRoot.Id.Should().Be(aggregateRootId);
        }

        [Test]
        public async Task TryLoad_LoadsAggregateWithAllEvents_WhenStreamExistsAndHasEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            var event1Version = Fixture.Create<long>();
            var event2Version = Fixture.Create<long>();
            var event1 = Fixture
                .Build<TestAggregateChanged>()
                .With(de => de.Metadata, new Metadata
                {
                    StorageVersion = event1Version
                })
                .Create();
            var event2 = Fixture
                .Build<TestAggregateChanged>()
                .With(de => de.Metadata, new Metadata
                {
                    StorageVersion = event2Version
                })
                .Create();

            var events = new List<DomainEvent>
            {
                event1,
                event2
            };

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(events);
            
            // act
            var aggregate = await Sut
                .TryLoad<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);
            aggregate.CurrentVersion.Should().Be(event2Version);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents<TestAggregateState>(events))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task TryLoad_LoadsAggregateWithoutEvents_WhenStreamExistsButHasNoEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<DomainEvent>>(new List<DomainEvent>()));

            // act
            var aggregate = await Sut
                .TryLoad<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);
            aggregate.CurrentVersion.Should().Be(0);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents<TestAggregateState>(A<ICollection<DomainEvent>>._))
                .MustHaveHappened(Repeated.Never);
        }

        [Test]
        public async Task TryLoad_ReturnsNull_WhenStreamDoesNotExist()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<DomainEvent>>(null));
            
            // act
            var aggregate = await Sut
                .TryLoad<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().BeNull();
        }

        [Test]
        public async Task Load_LoadsAggregateWithAllEvents_WhenStreamExistsAndHasEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            var event1Version = Fixture.Create<long>();
            var event2Version = Fixture.Create<long>();
            var event1 = Fixture
                .Build<TestAggregateChanged>()
                .With(de => de.Metadata, new Metadata
                {
                    StorageVersion = event1Version
                })
                .Create();
            var event2 = Fixture
                .Build<TestAggregateChanged>()
                .With(de => de.Metadata, new Metadata
                {
                    StorageVersion = event2Version
                })
                .Create();

            var events = new List<DomainEvent>
            {
                event1,
                event2
            };

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<DomainEvent>>(events));

            // act
            var aggregate = await Sut
                .Load<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);
            aggregate.CurrentVersion.Should().Be(event2Version);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents<TestAggregateState>(events))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Load_LoadsAggregateWithoutEvents_WhenStreamExistsButHasNoEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<DomainEvent>>(new List<DomainEvent>()));

            // act
            var aggregate = await Sut
                .Load<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);
            aggregate.CurrentVersion.Should().Be(0);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents<TestAggregateState>(A<ICollection<DomainEvent>>._))
                .MustHaveHappened(Repeated.Never);
        }

        [Test]
        public void Load_ThrowsException_WhenStreamDoesNotExist()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<DomainEvent>>(null));

            Func<Task> exceptionThrower = async () => await Sut
                .Load<TestAggregateRoot, TestAggregateState>(aggregateRootId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public async Task Save_StoresEvents_WhenStreamExists()
        {
            // arrange
            var event1 = Faker.Resolve<DomainEvent>();
            var event2 = Faker.Resolve<DomainEvent>();

            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.UncommittedEvents.Add(event1);
            aggregateRoot.UncommittedEvents.Add(event2);

            var expectedEvents = new List<DomainEvent>
            {
                event1,
                event2
            };

            // act
            await Sut
                .Save<TestAggregateRoot, TestAggregateState>(aggregateRoot)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventStore>()
                .StoreEvents(
                    aggregateRoot.Id,
                    aggregateRoot.CurrentVersion,
                    A<ICollection<DomainEvent>>.That.Matches(events => events.All(e => expectedEvents.Contains(e)))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Save_WillDispatchAllEvents_AfterTheyHaveBeenCorrectlySaved()
        {
            // arrange
            var event1 = Fixture.Create<TestAggregateCreated>();
            var event2 = Fixture.Create<TestAggregateChanged>();

            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.UncommittedEvents.Add(event1);
            aggregateRoot.UncommittedEvents.Add(event2);

            // act
            await Sut
                .Save<TestAggregateRoot, TestAggregateState>(aggregateRoot)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IEventDispatcher>()
                .RaiseEvent(A<DomainEvent>.That.Matches(de => de.Equals(event1))))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => Faker.Resolve<IEventDispatcher>()
                .RaiseEvent(A<DomainEvent>.That.Matches(de => de.Equals(event2))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Save_WillNotDispatchAnyEvent_WhenEventStoreThrowsException()
        {
            // arrange
            var event1 = Fixture.Create<TestAggregateCreated>();
            var event2 = Fixture.Create<TestAggregateChanged>();

            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            aggregateRoot.UncommittedEvents.Add(event1);
            aggregateRoot.UncommittedEvents.Add(event2);

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .StoreEvents(
                    aggregateRoot.Id,
                    aggregateRoot.CurrentVersion,
                    A<ICollection<DomainEvent>>._))
                .Throws<InvalidOperationException>();

            Func<Task> expectionThrower = async () =>
                await Sut
                    .Save<TestAggregateRoot, TestAggregateState>(aggregateRoot)
                    .ConfigureAwait(false);

            // act & assert
            expectionThrower
                .ShouldThrow<InvalidOperationException>();

            A.CallTo(() => Faker.Resolve<IEventDispatcher>()
                .RaiseEvent(A<DomainEvent>._))
                .MustHaveHappened(Repeated.Never);
        }
    }
}