using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Data.Contracts;
using Playground.Domain.Persistence.PostgreSQL.Commands;
using Playground.Domain.Persistence.PostgreSQL.Queries;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.PostgreSQL.UnitTests
{
    public class EventRepositoryTests: TestBase
    {
        private IEventRepository _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = Faker.Resolve<EventRepository>();
        }

        [Test]
        public async Task GetAll_ObtainsAllEvents_WhenOpensConnection()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Guid.NewGuid();

            IEnumerable<Event> expected = Fixture
                .CreateMany<Event>()
                .ToList();

            A.CallTo(() => fakeConnection
                .ExecuteQueryMultiple<Event>(
                    A<string>._,
                    A<object>.That.Matches(p => ((GetAllEventsQuery) p).StreamId == streamId)))
                .Returns(Task.FromResult(expected));

            // act
            var actual = await _sut
                .GetAll(streamId)
                .ConfigureAwait(false);

            // assert
            actual.ShouldAllBeEquivalentTo(expected);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetAll_ReturnsEmptyList_WhenOpensConnectionAndHasNoEvents()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Guid.NewGuid();

            IEnumerable<Event> expected = new List<Event>();

            A.CallTo(() => fakeConnection
                .ExecuteQueryMultiple<Event>(
                    A<string>._,
                    A<object>.That.Matches(p => ((GetAllEventsQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(expected));

            // act
            var actual = await _sut
                .GetAll(streamId)
                .ConfigureAwait(false);

            // assert
            actual.Should().BeEmpty();

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetAll_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await _sut
                .GetAll(Guid.Empty)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Get_ObtainsSingleEvent_WhenOpensConnection()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            var eventId = Fixture.Create<long>();

            var expectedEvent = Fixture
                .Create<Event>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<Event>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery) p).StreamId == streamId
                        && ((GetEventQuery) p).EventId == eventId)))
                .Returns(Task.FromResult(expectedEvent));

            // act
            var actualEvent = await _sut
                .Get(streamId, eventId)
                .ConfigureAwait(false);

            // assert
            actualEvent.ShouldBeEquivalentTo(expectedEvent);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Get_ReturnsNull_WhenOpensConnectionAndEventDoesNotExist()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            var eventId = Fixture.Create<long>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<Event>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery)p).StreamId == streamId
                        && ((GetEventQuery)p).EventId == eventId)))
                .Returns(Task.FromResult<Event>(null));

            // act
            var actualEvent = await _sut
                .Get(streamId, eventId)
                .ConfigureAwait(false);

            // assert
            actualEvent.Should().BeNull();

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Get_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await _sut
                .Get(Guid.Empty, Fixture.Create<long>())
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [TestCase(0L)]
        [TestCase(-4L)]
        public void Get_ThrowsException_WhenEventIdIsInvalid(long invalidEventId)
        {
            // arrange
            Func<Task> exceptionThrower = async () => await _sut
                .Get(Fixture.Create<Guid>(), invalidEventId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task Add_ExecutesCommand_WhenOpensConnection()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            var eventToAdd = Fixture
                .Create<Event>();

            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((AddEventCommand)p).StreamId == streamId)))
                .Returns(Task.FromResult(1));

            // act
            await _sut
                .Add(streamId, eventToAdd)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((AddEventCommand) p).StreamId == streamId)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
