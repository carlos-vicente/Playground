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

            var created = new TestAggregateCreated
            {
                Metadata = new Metadata(aggregateRootId),
                Name = Fixture.Create<string>()
            };

            var changed = new TestAggregateChanged
            {
                Metadata = new Metadata(aggregateRootId),
                NewName = Fixture.Create<string>()
            };

            A.CallTo(() => Faker.Resolve<IEventStore>()
                .LoadAllEvents(aggregateRootId))
                .Returns(Task.FromResult<ICollection<IEvent>>(new List<IEvent>
                {
                    created,
                    changed
                }));

            
            // act
            var aggregate = await _sut
                .TryLoad<TestAggregateRoot>(aggregateRootId)
                .ConfigureAwait(false);

            // assert
        }

        [Test]
        public void TryLoad_LoadsAggregateWithoutEvents_WhenStreamExistsButHasNoEvents()
        {

        }

        [Test]
        public void TryLoad_ReturnsNull_WhenStreamDoesNotExist()
        {

        }
    }
}