using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Core.Validation;
using Playground.Data.Contracts;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL.Commands;
using Playground.Domain.Persistence.PostgreSQL.Queries;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.PostgreSQL.UnitTests
{
    public class EventRepositoryTests: TestBaseWithSut<EventRepository>
    {
        [Test]
        public async Task GetAll_ObtainsAllEvents_WhenOpensConnection()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Guid.NewGuid();

            IEnumerable<StoredEvent> expected = Fixture
                .CreateMany<StoredEvent>()
                .ToList();

            A.CallTo(() => fakeConnection
                .ExecuteQueryMultiple<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p => ((GetAllEventsQuery) p).StreamId == streamId)))
                .Returns(Task.FromResult(expected));

            // act
            var actual = await Sut
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

            IEnumerable<StoredEvent> expected = new List<StoredEvent>();

            A.CallTo(() => fakeConnection
                .ExecuteQueryMultiple<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p => ((GetAllEventsQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(expected));

            // act
            var actual = await Sut
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
            Func<Task> exceptionThrower = async () => await Sut
                .GetAll(default(Guid))
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
                .Create<StoredEvent>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery) p).StreamId == streamId
                        && ((GetEventQuery) p).EventId == eventId)))
                .Returns(Task.FromResult(expectedEvent));

            // act
            var actualEvent = await Sut
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
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery)p).StreamId == streamId
                        && ((GetEventQuery)p).EventId == eventId)))
                .Returns(Task.FromResult<StoredEvent>(null));

            // act
            var actualEvent = await Sut
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
            Func<Task> exceptionThrower = async () => await Sut
                .Get(default(Guid), Fixture.Create<long>())
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
            Func<Task> exceptionThrower = async () => await Sut
                .Get(Fixture.Create<Guid>(), invalidEventId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task GetLastEvent_ObtainsSingleEvent_WhenOpensConnection()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            
            var expectedEvent = Fixture
                .Create<StoredEvent>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(expectedEvent));

            // act
            var actualEvent = await Sut
                .GetLastEvent(streamId)
                .ConfigureAwait(false);

            // assert
            actualEvent.ShouldBeEquivalentTo(expectedEvent);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetLastEvent_ReturnsNull_WhenOpensConnectionAndEventDoesNotExist()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            
            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult<StoredEvent>(null));

            // act
            var actualEvent = await Sut
                .GetLastEvent(streamId)
                .ConfigureAwait(false);

            // assert
            actualEvent.Should().BeNull();

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetLastEvent_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .GetLastEvent(default(Guid))
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [TestCase(3L, 4L)]
        [TestCase(3L, 10L)]
        public async Task Add_ExecutesCommand_WhenOpensConnectionAndTheNewEventHasABiggerEventId(
            long lastEventId,
            long newEventId)
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            var eventToAdd = Fixture
                .Build<StoredEvent>()
                .With(e => e.EventId, newEventId)
                .Create();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery) p).StreamId == streamId)))
                .Returns(Task.FromResult(Fixture
                    .Build<StoredEvent>()
                    .With(e => e.EventId, lastEventId)
                    .Create()));

            // act
            await Sut
                .Add(streamId, eventToAdd)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((AddEventCommand) p).StreamId == streamId
                        && ((AddEventCommand)p).EventId == eventToAdd.EventId
                        && ((AddEventCommand)p).TypeName == eventToAdd.TypeName
                        && ((AddEventCommand)p).OccurredOn == eventToAdd.OccurredOn
                        && ((AddEventCommand)p).EventBody == eventToAdd.EventBody)))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestCase(3L, 3L)]
        [TestCase(3L, 1L)]
        public void Add_ThrowsException_WhenOpensConnectionAndTheNewEventDoesNotHaveABiggerEventId(
            long lastEventId,
            long newEventId)
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            var eventToAdd = Fixture
                .Build<StoredEvent>()
                .With(e => e.EventId, newEventId)
                .Create();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(Fixture
                    .Build<StoredEvent>()
                    .With(e => e.EventId, lastEventId)
                    .Create()));

            Func<Task> exceptionThrower = async () => await Sut
                .Add(streamId, eventToAdd)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>()
                .And
                .Message
                .Should()
                .Contain($"EventId {newEventId} must be bigger than last EventId stored {lastEventId}");

            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>._))
                .MustHaveHappened(Repeated.Never);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Add_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .Add(default(Guid), Fixture.Create<StoredEvent>())
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Contain("streamId");
        }

        [Test]
        public void Add_ThrowsException_WhenEventIsNull()
        {
            // arrange
            var fakeValidator = Faker
                .Resolve<IValidator<StoredEvent>>();

            A.CallTo(() => Faker.Resolve<IValidatorFactory>()
                .CreateValidator<StoredEvent>())
                .Returns(fakeValidator);

            A.CallTo(() => fakeValidator
                .Validate(A<StoredEvent>._))
                .Throws<ValidationException>();

            Func<Task> exceptionThrower = async () => await Sut
                .Add(Guid.NewGuid(), null as StoredEvent)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ValidationException>();
        }

        [Test]
        public void Add_ThrowsException_WhenEventIsInvalid()
        {
            // arrange
            var @event = Fixture
                .Create<StoredEvent>();

            var fakeValidator = Faker
                .Resolve<IValidator<StoredEvent>>();

            A.CallTo(() => Faker.Resolve<IValidatorFactory>()
                .CreateValidator<StoredEvent>())
                .Returns(fakeValidator);

            A.CallTo(() => fakeValidator
                .Validate(@event))
                .Throws<ValidationException>();

            Func<Task> exceptionThrower = async () => await Sut
                .Add(Guid.NewGuid(), @event)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ValidationException>();
        }

        [TestCase(3L, 4L)]
        [TestCase(3L, 10L)]
        public async Task AddMultiple_ExecutesCommand_WhenOpensConnectionAndTheFirstNewEventHasABiggerEventId(
            long lastEventId,
            long firstNewEventId)
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            var event1 = Fixture
                .Build<StoredEvent>()
                .With(e => e.EventId, firstNewEventId)
                .Create();

            var event2 = Fixture
                .Build<StoredEvent>()
                .With(e => e.EventId, firstNewEventId + 1)
                .Create();

            var events = new List<StoredEvent>
            {
                event2,
                event1
            };

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(Fixture
                    .Build<StoredEvent>()
                    .With(e => e.EventId, lastEventId)
                    .Create()));

            // act
            await Sut
                .Add(streamId, events)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((AddEventCommand)p).StreamId == streamId
                        && ((AddEventCommand)p).EventId == event1.EventId
                        && ((AddEventCommand)p).TypeName == event1.TypeName
                        && ((AddEventCommand)p).OccurredOn == event1.OccurredOn
                        && ((AddEventCommand)p).EventBody == event1.EventBody)))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((AddEventCommand)p).StreamId == streamId
                        && ((AddEventCommand)p).EventId == event2.EventId
                        && ((AddEventCommand)p).TypeName == event2.TypeName
                        && ((AddEventCommand)p).OccurredOn == event2.OccurredOn
                        && ((AddEventCommand)p).EventBody == event2.EventBody)))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestCase(3L, 3L)]
        [TestCase(3L, 1L)]
        public void AddMultiple_ThrowsException_WhenOpensConnectionAndTheNewEventDoesNotHaveABiggerEventId(
            long lastEventId,
            long newEventId)
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            var events = new List<StoredEvent>()
            {
                Fixture
                    .Build<StoredEvent>()
                    .With(e => e.EventId, newEventId)
                    .Create()
            };

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(Fixture
                    .Build<StoredEvent>()
                    .With(e => e.EventId, lastEventId)
                    .Create()));

            Func<Task> exceptionThrower = async () => await Sut
                .Add(streamId, events)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>()
                .And
                .Message
                .Should()
                .Contain($"EventId {newEventId} must be bigger than last EventId stored {lastEventId}");

            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>._))
                .MustHaveHappened(Repeated.Never);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void AddMultiple_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .Add(default(Guid), Fixture.CreateMany<StoredEvent>().ToList())
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Contain("streamId");
        }

        [Test]
        public void AddMultiple_ThrowsException_WhenEventIsNull()
        {
            // arrange
            var fakeValidator = Faker
                .Resolve<IValidator<StoredEvent>>();

            A.CallTo(() => Faker.Resolve<IValidatorFactory>()
                .CreateValidator<StoredEvent>())
                .Returns(fakeValidator);

            A.CallTo(() => fakeValidator
                .ValidateAll(A<ICollection<StoredEvent>>._))
                .Throws<ValidationException>();

            Func<Task> exceptionThrower = async () => await Sut
                .Add(Guid.NewGuid(), null as ICollection<StoredEvent>)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ValidationException>();
        }

        [Test]
        public void AddMultiple_ThrowsException_WhenEventIsInvalid()
        {
            // arrange
            var events = Fixture
                .CreateMany<StoredEvent>()
                .ToList();

            var fakeValidator = Faker
                .Resolve<IValidator<StoredEvent>>();

            A.CallTo(() => Faker.Resolve<IValidatorFactory>()
                .CreateValidator<StoredEvent>())
                .Returns(fakeValidator);

            A.CallTo(() => fakeValidator
                .ValidateAll(events))
                .Throws<ValidationException>();

            Func<Task> exceptionThrower = async () => await Sut
                .Add(Guid.NewGuid(), events)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ValidationException>();
        }

        [Test]
        public async Task Remove_ExecutesCommand_WhenEventExists()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            var eventId = Fixture.Create<long>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery)p).StreamId == streamId
                        && ((GetEventQuery)p).EventId == eventId)))
                .Returns(Task.FromResult(Fixture.Create<StoredEvent>()));

            // act
            await Sut
                .Remove(streamId, eventId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((RemoveEventCommand)p).StreamId == streamId
                        && ((RemoveEventCommand)p).EventId == eventId)))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Remove_DoesNotExecuteCommand_WhenEventDoesNotExists()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();
            var eventId = Fixture.Create<long>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetEventQuery)p).StreamId == streamId
                        && ((GetEventQuery)p).EventId == eventId)))
                .Returns(Task.FromResult<StoredEvent>(null));

            // act
            await Sut
                .Remove(streamId, eventId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>._))
                .MustHaveHappened(Repeated.Never);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Remove_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .Remove(default(Guid), Fixture.Create<long>())
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [TestCase(0L)]
        [TestCase(-4L)]
        public void Remove_ThrowsException_WhenEventIdIsInvalid(long invalidEventId)
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .Remove(Fixture.Create<Guid>(), invalidEventId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task RemoveMultiple_ExecutesCommand_WhenStreamContainsEvents()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult(Fixture.Create<StoredEvent>()));

            // act
            await Sut
                .Remove(streamId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((RemoveAllEventsCommand)p).StreamId == streamId)))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task RemoveMultiple_DoesNotExecuteCommand_WhenStreamHasNoEvents()
        {
            // arrange
            var fakeConnection = Faker.Resolve<IConnection>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(fakeConnection);

            var streamId = Fixture.Create<Guid>();

            A.CallTo(() => fakeConnection
                .ExecuteQuerySingle<StoredEvent>(
                    A<string>._,
                    A<object>.That.Matches(p =>
                        ((GetLastEventQuery)p).StreamId == streamId)))
                .Returns(Task.FromResult<StoredEvent>(null));

            // act
            await Sut
                .Remove(streamId)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeConnection
                .ExecuteCommand(
                    A<string>._,
                    A<object>._))
                .MustHaveHappened(Repeated.Never);

            A.CallTo(() => fakeConnection.Dispose())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void RemoveMultiple_ThrowsException_WhenStreamIdIsEmpty()
        {
            // arrange
            Func<Task> exceptionThrower = async () => await Sut
                .Remove(default(Guid))
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }
    }
}
