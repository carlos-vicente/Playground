using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.UnitTests.TestModel;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.UnitTests
{
    public class AggregateContextTests : TestBase
    {
        private AggregateContext _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<AggregateContext>();
        }

        [Test]
        public async Task Create_ReturnsNewInstanceWithId_WhenStreamIsSuccessfullyCreated()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .CreateEventStream(aggregateRootId))
                .Returns(Task.FromResult(true));

            // act
            var aggregateRoot = await _sut
                .Create<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(true);

            // assert
            aggregateRoot.Should().NotBeNull();
            aggregateRoot.Id.Should().Be(aggregateRootId);
        }

        [Test]
        public void Create_ThrowsException_WhenStreamIsNotCreated()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .CreateEventStream(aggregateRootId))
                .Returns(Task.FromResult(false));

            Func<Task> exceptionThrower = async () => await _sut
                .Create<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(true);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public async Task TryLoad_LoadsAggregateWithAllEvents_WhenStreamExistsAndHasEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            var event1 = Faker.Resolve<IEvent>();
            var event2 = Faker.Resolve<IEvent>();

            var events = new List<IEvent>
            {
                event1,
                event2
            };

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(events));
            
            // act
            var aggregate = await _sut
                .TryLoad<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents(aggregate, events))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task TryLoad_LoadsAggregateWithoutEvents_WhenStreamExistsButHasNoEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(new List<IEvent>()));

            // act
            var aggregate = await _sut
                .TryLoad<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents(A<TestAggregateRoot>._, A<ICollection<IEvent>>._))
                .MustHaveHappened(Repeated.Never);
        }

        [Test]
        public async Task TryLoad_ReturnsNull_WhenStreamDoesNotExist()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(null));
            
            // act
            var aggregate = await _sut
                .TryLoad<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().BeNull();
        }

        [Test]
        public async Task Load_LoadsAggregateWithAllEvents_WhenStreamExistsAndHasEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            var event1 = Faker.Resolve<IEvent>();
            var event2 = Faker.Resolve<IEvent>();

            var events = new List<IEvent>
            {
                event1,
                event2
            };

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(events));

            // act
            var aggregate = await _sut
                .Load<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents(aggregate, events))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Load_LoadsAggregateWithoutEvents_WhenStreamExistsButHasNoEvents()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(new List<IEvent>()));

            // act
            var aggregate = await _sut
                .Load<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
            aggregate.Should().NotBeNull();
            aggregate.Id.Should().Be(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateHydrator>()
                .HydrateAggregateWithEvents(A<TestAggregateRoot>._, A<ICollection<IEvent>>._))
                .MustHaveHappened(Repeated.Never);
        }

        [Test]
        public void Load_ThrowsException_WhenStreamDoesNotExist()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(null));

            Func<Task> exceptionThrower = async () => await _sut
                .Load<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Save_StoresEvents_WhenStreamExists()
        {
            Assert.Inconclusive();
        }
    }
}