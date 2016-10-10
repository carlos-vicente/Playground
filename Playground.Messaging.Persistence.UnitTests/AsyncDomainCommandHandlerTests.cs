using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Domain.Persistence;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Messaging.Persistence.UnitTests
{
    public class AsyncDomainCommandHandlerTests : SimpleTestBase
    {
        private TestHandler _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<TestHandler>();
        }

        [Test]
        public async Task Handle_WillCreateANewAggregate_WhenThereIsNoneWithTheGivenIdentifier()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();
            var aggregateRoot = Fixture.Create<Aggregate>();
            var command = new Command(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .TryLoad<Aggregate, AggregateState>(aggregateRootId))
                .Returns(null as Aggregate);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .Create<Aggregate, AggregateState>(aggregateRootId))
                .Returns(aggregateRoot);

            // act
            await _sut
                .Handle(command)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .Create<Aggregate, AggregateState>(aggregateRootId))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Handle_WillNotCreateANewAggregate_WhenThereIsOneWithTheGivenIdentifier()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();
            var aggregateRoot = Fixture.Create<Aggregate>();
            var command = new Command(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .TryLoad<Aggregate, AggregateState>(aggregateRootId))
                .Returns(aggregateRoot);

            // act
            await _sut
                .Handle(command)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .Create<Aggregate, AggregateState>(A<Guid>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_WillProcessCommandOnAggregate()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();
            var aggregateRoot = Fixture.Create<Aggregate>();
            var command = new Command(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .TryLoad<Aggregate, AggregateState>(aggregateRootId))
                .Returns(aggregateRoot);

            // act
            await _sut
                .Handle(command)
                .ConfigureAwait(false);

            // assert
            _sut
                .CalledWithCommand
                .Should()
                .Be(command);
            _sut
                .CalledWithAggregate
                .Should()
                .Be(aggregateRoot);
        }

        [Test]
        public async Task Handle_WillSaveAggregate_WhenItHasBeenProcessed()
        {
            // arrange
            var aggregateRootId = Fixture.Create<Guid>();
            var aggregateRoot = Fixture.Create<Aggregate>();
            var command = new Command(aggregateRootId);

            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .TryLoad<Aggregate, AggregateState>(aggregateRootId))
                .Returns(aggregateRoot);

            // act
            await _sut
                .Handle(command)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IAggregateContext>()
                .Save<Aggregate, AggregateState>(aggregateRoot))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}