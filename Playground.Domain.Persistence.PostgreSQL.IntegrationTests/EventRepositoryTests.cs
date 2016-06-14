using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;
using Playground.Core.Validation;
using Playground.Data.Contracts;
using Playground.Domain.Persistence.Events;
using Playground.Domain.Persistence.PostgreSQL.Commands;
using Playground.Domain.Persistence.PostgreSQL.IntegrationTests.Postgresql;
using Playground.Domain.Persistence.PostgreSQL.Queries;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.PostgreSQL.IntegrationTests
{
    //public class EventRepositoryTests: TestBaseWithSut<EventRepository>
    //{
    //    [Test]
    //    public async Task GetAll_ObtainsAllEvents_WhenOpensConnection()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Guid.NewGuid();

    //        IEnumerable<StoredEvent> expected = Fixture
    //            .CreateMany<StoredEvent>()
    //            .ToList();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQueryMultiple<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p => ((GetAllEventsQuery) p).streamId == streamId)))
    //            .Returns(Task.FromResult(expected));

    //        // act
    //        var actual = await Sut
    //            .GetAll(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actual.ShouldAllBeEquivalentTo(expected);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public async Task GetAll_ReturnsEmptyList_WhenOpensConnectionAndHasNoEvents()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Guid.NewGuid();

    //        IEnumerable<StoredEvent> expected = new List<StoredEvent>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQueryMultiple<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p => ((GetAllEventsQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult(expected));

    //        // act
    //        var actual = await Sut
    //            .GetAll(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actual.Should().BeEmpty();

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void GetAll_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .GetAll(default(Guid))
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    [Test]
    //    public async Task Get_ObtainsSingleEvent_WhenOpensConnection()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();
    //        var eventId = Fixture.Create<long>();

    //        var expectedEvent = Fixture
    //            .Create<StoredEvent>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetEventQuery) p).StreamId == streamId
    //                    && ((GetEventQuery) p).EventId == eventId)))
    //            .Returns(Task.FromResult(expectedEvent));

    //        // act
    //        var actualEvent = await Sut
    //            .Get(streamId, eventId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actualEvent.ShouldBeEquivalentTo(expectedEvent);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public async Task Get_ReturnsNull_WhenOpensConnectionAndEventDoesNotExist()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();
    //        var eventId = Fixture.Create<long>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetEventQuery)p).StreamId == streamId
    //                    && ((GetEventQuery)p).EventId == eventId)))
    //            .Returns(Task.FromResult<StoredEvent>(null));

    //        // act
    //        var actualEvent = await Sut
    //            .Get(streamId, eventId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actualEvent.Should().BeNull();

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void Get_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Get(default(Guid), Fixture.Create<long>())
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    [TestCase(0L)]
    //    [TestCase(-4L)]
    //    public void Get_ThrowsException_WhenEventIdIsInvalid(long invalidEventId)
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Get(Fixture.Create<Guid>(), invalidEventId)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    [Test]
    //    public async Task GetLastEvent_ObtainsSingleEvent_WhenOpensConnection()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        var expectedEvent = Fixture
    //            .Create<StoredEvent>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult(expectedEvent));

    //        // act
    //        var actualEvent = await Sut
    //            .GetLastEvent(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actualEvent.ShouldBeEquivalentTo(expectedEvent);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public async Task GetLastEvent_ReturnsNull_WhenOpensConnectionAndEventDoesNotExist()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult<StoredEvent>(null));

    //        // act
    //        var actualEvent = await Sut
    //            .GetLastEvent(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        actualEvent.Should().BeNull();

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void GetLastEvent_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .GetLastEvent(default(Guid))
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    //[TestCase(3L, 4L)]
    //    //[TestCase(3L, 10L)]
    //    //public async Task Add_ExecutesCommand_WhenOpensConnectionAndTheNewEventHasABiggerEventId(
    //    //    long lastEventId,
    //    //    long newEventId)
    //    //{
    //    //    // arrange
    //    //    var fakeConnection = Faker.Resolve<IConnection>();

    //    //    A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //    //        .CreateConnection())
    //    //        .Returns(fakeConnection);

    //    //    var streamId = Fixture.Create<Guid>();

    //    //    var eventToAdd = Fixture
    //    //        .Build<StoredEvent>()
    //    //        .With(e => e.EventId, newEventId)
    //    //        .Create();

    //    //    A.CallTo(() => fakeConnection
    //    //        .ExecuteQuerySingle<StoredEvent>(
    //    //            A<string>._,
    //    //            A<object>.That.Matches(p =>
    //    //                ((GetLastEventQuery) p).streamId == streamId)))
    //    //        .Returns(Task.FromResult(Fixture
    //    //            .Build<StoredEvent>()
    //    //            .With(e => e.EventId, lastEventId)
    //    //            .Create()));

    //    //    // act
    //    //    await Sut
    //    //        .Add(streamId, eventToAdd)
    //    //        .ConfigureAwait(false);

    //    //    // assert
    //    //    A.CallTo(() => fakeConnection
    //    //        .ExecuteCommand(
    //    //            A<string>._,
    //    //            A<object>.That.Matches(p =>
    //    //                ((AddEventsCommand) p).StreamId == streamId
    //    //                && ((AddEventsCommand)p).EventId == eventToAdd.EventId
    //    //                && ((AddEventsCommand)p).TypeName == eventToAdd.TypeName
    //    //                && ((AddEventsCommand)p).OccurredOn == eventToAdd.OccurredOn
    //    //                && ((AddEventsCommand)p).EventBody == eventToAdd.EventBody)))
    //    //        .MustHaveHappened(Repeated.Exactly.Once);

    //    //    A.CallTo(() => fakeConnection.Dispose())
    //    //        .MustHaveHappened(Repeated.Exactly.Once);
    //    //}

    //    [TestCase(3L, 3L)]
    //    [TestCase(3L, 1L)]
    //    public void Add_ThrowsException_WhenOpensConnectionAndTheNewEventDoesNotHaveABiggerEventId(
    //        long lastEventId,
    //        long newEventId)
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        var eventToAdd = Fixture
    //            .Build<StoredEvent>()
    //            .With(e => e.EventId, newEventId)
    //            .Create();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult(Fixture
    //                .Build<StoredEvent>()
    //                .With(e => e.EventId, lastEventId)
    //                .Create()));

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(streamId, eventToAdd)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<InvalidOperationException>()
    //            .And
    //            .Message
    //            .Should()
    //            .Contain($"EventId {newEventId} must be bigger than last EventId stored {lastEventId}");

    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>._))
    //            .MustHaveHappened(Repeated.Never);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void Add_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(default(Guid), Fixture.Create<StoredEvent>())
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>()
    //            .And
    //            .ParamName
    //            .Should()
    //            .Contain("streamId");
    //    }

    //    [Test]
    //    public void Add_ThrowsException_WhenEventIsNull()
    //    {
    //        // arrange
    //        var fakeValidator = Faker
    //            .Resolve<IValidator<StoredEvent>>();

    //        A.CallTo(() => Faker.Resolve<IValidatorFactory>()
    //            .CreateValidator<StoredEvent>())
    //            .Returns(fakeValidator);

    //        A.CallTo(() => fakeValidator
    //            .Validate(A<StoredEvent>._))
    //            .Throws<ValidationException>();

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(Guid.NewGuid(), null as StoredEvent)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ValidationException>();
    //    }

    //    [Test]
    //    public void Add_ThrowsException_WhenEventIsInvalid()
    //    {
    //        // arrange
    //        var @event = Fixture
    //            .Create<StoredEvent>();

    //        var fakeValidator = Faker
    //            .Resolve<IValidator<StoredEvent>>();

    //        A.CallTo(() => Faker.Resolve<IValidatorFactory>()
    //            .CreateValidator<StoredEvent>())
    //            .Returns(fakeValidator);

    //        A.CallTo(() => fakeValidator
    //            .Validate(@event))
    //            .Throws<ValidationException>();

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(Guid.NewGuid(), @event)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ValidationException>();
    //    }

    //    //[TestCase(3L, 4L)]
    //    //[TestCase(3L, 10L)]
    //    //public async Task AddMultiple_ExecutesCommand_WhenOpensConnectionAndTheFirstNewEventHasABiggerEventId(
    //    //    long lastEventId,
    //    //    long firstNewEventId)
    //    //{
    //    //    // arrange
    //    //    var fakeConnection = Faker.Resolve<IConnection>();

    //    //    A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //    //        .CreateConnection())
    //    //        .Returns(fakeConnection);

    //    //    var streamId = Fixture.Create<Guid>();

    //    //    var event1 = Fixture
    //    //        .Build<StoredEvent>()
    //    //        .With(e => e.EventId, firstNewEventId)
    //    //        .Create();

    //    //    var event2 = Fixture
    //    //        .Build<StoredEvent>()
    //    //        .With(e => e.EventId, firstNewEventId + 1)
    //    //        .Create();

    //    //    var events = new List<StoredEvent>
    //    //    {
    //    //        event2,
    //    //        event1
    //    //    };

    //    //    A.CallTo(() => fakeConnection
    //    //        .ExecuteQuerySingle<StoredEvent>(
    //    //            A<string>._,
    //    //            A<object>.That.Matches(p =>
    //    //                ((GetLastEventQuery)p).streamId == streamId)))
    //    //        .Returns(Task.FromResult(Fixture
    //    //            .Build<StoredEvent>()
    //    //            .With(e => e.EventId, lastEventId)
    //    //            .Create()));

    //    //    // act
    //    //    await Sut
    //    //        .Add(streamId, events)
    //    //        .ConfigureAwait(false);

    //    //    // assert
    //    //    A.CallTo(() => fakeConnection
    //    //        .ExecuteCommand(
    //    //            A<string>._,
    //    //            A<object>.That.Matches(p =>
    //    //                ((AddEventsCommand)p).StreamId == streamId
    //    //                && ((AddEventsCommand)p).EventId == event1.EventId
    //    //                && ((AddEventsCommand)p).TypeName == event1.TypeName
    //    //                && ((AddEventsCommand)p).OccurredOn == event1.OccurredOn
    //    //                && ((AddEventsCommand)p).EventBody == event1.EventBody)))
    //    //        .MustHaveHappened(Repeated.Exactly.Once);
    //    //    A.CallTo(() => fakeConnection
    //    //        .ExecuteCommand(
    //    //            A<string>._,
    //    //            A<object>.That.Matches(p =>
    //    //                ((AddEventsCommand)p).StreamId == streamId
    //    //                && ((AddEventsCommand)p).EventId == event2.EventId
    //    //                && ((AddEventsCommand)p).TypeName == event2.TypeName
    //    //                && ((AddEventsCommand)p).OccurredOn == event2.OccurredOn
    //    //                && ((AddEventsCommand)p).EventBody == event2.EventBody)))
    //    //        .MustHaveHappened(Repeated.Exactly.Once);

    //    //    A.CallTo(() => fakeConnection.Dispose())
    //    //        .MustHaveHappened(Repeated.Exactly.Once);
    //    //}

    //    [TestCase(3L, 3L)]
    //    [TestCase(3L, 1L)]
    //    public void AddMultiple_ThrowsException_WhenOpensConnectionAndTheNewEventDoesNotHaveABiggerEventId(
    //        long lastEventId,
    //        long newEventId)
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        var events = new List<StoredEvent>()
    //        {
    //            Fixture
    //                .Build<StoredEvent>()
    //                .With(e => e.EventId, newEventId)
    //                .Create()
    //        };

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult(Fixture
    //                .Build<StoredEvent>()
    //                .With(e => e.EventId, lastEventId)
    //                .Create()));

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(streamId, events)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<InvalidOperationException>()
    //            .And
    //            .Message
    //            .Should()
    //            .Contain($"EventId {newEventId} must be bigger than last EventId stored {lastEventId}");

    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>._))
    //            .MustHaveHappened(Repeated.Never);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void AddMultiple_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(default(Guid), Fixture.CreateMany<StoredEvent>().ToList())
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>()
    //            .And
    //            .ParamName
    //            .Should()
    //            .Contain("streamId");
    //    }

    //    [Test]
    //    public void AddMultiple_ThrowsException_WhenEventIsNull()
    //    {
    //        // arrange
    //        var fakeValidator = Faker
    //            .Resolve<IValidator<StoredEvent>>();

    //        A.CallTo(() => Faker.Resolve<IValidatorFactory>()
    //            .CreateValidator<StoredEvent>())
    //            .Returns(fakeValidator);

    //        A.CallTo(() => fakeValidator
    //            .ValidateAll(A<ICollection<StoredEvent>>._))
    //            .Throws<ValidationException>();

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(Guid.NewGuid(), null as ICollection<StoredEvent>)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ValidationException>();
    //    }

    //    [Test]
    //    public void AddMultiple_ThrowsException_WhenEventIsInvalid()
    //    {
    //        // arrange
    //        var events = Fixture
    //            .CreateMany<StoredEvent>()
    //            .ToList();

    //        var fakeValidator = Faker
    //            .Resolve<IValidator<StoredEvent>>();

    //        A.CallTo(() => Faker.Resolve<IValidatorFactory>()
    //            .CreateValidator<StoredEvent>())
    //            .Returns(fakeValidator);

    //        A.CallTo(() => fakeValidator
    //            .ValidateAll(events))
    //            .Throws<ValidationException>();

    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Add(Guid.NewGuid(), events)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ValidationException>();
    //    }

    //    [Test]
    //    public async Task Remove_ExecutesCommand_WhenEventExists()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();
    //        var eventId = Fixture.Create<long>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetEventQuery)p).StreamId == streamId
    //                    && ((GetEventQuery)p).EventId == eventId)))
    //            .Returns(Task.FromResult(Fixture.Create<StoredEvent>()));

    //        // act
    //        await Sut
    //            .Remove(streamId, eventId)
    //            .ConfigureAwait(false);

    //        // assert
    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((RemoveEventCommand)p).StreamId == streamId
    //                    && ((RemoveEventCommand)p).EventId == eventId)))
    //            .MustHaveHappened(Repeated.Exactly.Once);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public async Task Remove_DoesNotExecuteCommand_WhenEventDoesNotExists()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();
    //        var eventId = Fixture.Create<long>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetEventQuery)p).StreamId == streamId
    //                    && ((GetEventQuery)p).EventId == eventId)))
    //            .Returns(Task.FromResult<StoredEvent>(null));

    //        // act
    //        await Sut
    //            .Remove(streamId, eventId)
    //            .ConfigureAwait(false);

    //        // assert
    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>._))
    //            .MustHaveHappened(Repeated.Never);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void Remove_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Remove(default(Guid), Fixture.Create<long>())
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    [TestCase(0L)]
    //    [TestCase(-4L)]
    //    public void Remove_ThrowsException_WhenEventIdIsInvalid(long invalidEventId)
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Remove(Fixture.Create<Guid>(), invalidEventId)
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }

    //    [Test]
    //    public async Task RemoveMultiple_ExecutesCommand_WhenStreamContainsEvents()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult(Fixture.Create<StoredEvent>()));

    //        // act
    //        await Sut
    //            .Remove(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((RemoveAllEventsCommand)p).StreamId == streamId)))
    //            .MustHaveHappened(Repeated.Exactly.Once);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public async Task RemoveMultiple_DoesNotExecuteCommand_WhenStreamHasNoEvents()
    //    {
    //        // arrange
    //        var fakeConnection = Faker.Resolve<IConnection>();

    //        A.CallTo(() => Faker.Resolve<IConnectionFactory>()
    //            .CreateConnection())
    //            .Returns(fakeConnection);

    //        var streamId = Fixture.Create<Guid>();

    //        A.CallTo(() => fakeConnection
    //            .ExecuteQuerySingle<StoredEvent>(
    //                A<string>._,
    //                A<object>.That.Matches(p =>
    //                    ((GetLastEventQuery)p).streamId == streamId)))
    //            .Returns(Task.FromResult<StoredEvent>(null));

    //        // act
    //        await Sut
    //            .Remove(streamId)
    //            .ConfigureAwait(false);

    //        // assert
    //        A.CallTo(() => fakeConnection
    //            .ExecuteCommand(
    //                A<string>._,
    //                A<object>._))
    //            .MustHaveHappened(Repeated.Never);

    //        A.CallTo(() => fakeConnection.Dispose())
    //            .MustHaveHappened(Repeated.Exactly.Once);
    //    }

    //    [Test]
    //    public void RemoveMultiple_ThrowsException_WhenStreamIdIsEmpty()
    //    {
    //        // arrange
    //        Func<Task> exceptionThrower = async () => await Sut
    //            .Remove(default(Guid))
    //            .ConfigureAwait(false);

    //        // act/assert
    //        exceptionThrower
    //            .ShouldThrow<ArgumentException>();
    //    }
    //}

    public class EventRepositoryTests : SimpleTestBase
    {
        private EventRepository _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = new EventRepository(DatabaseHelper.GetConnectionStringBuilder());
        }

        [OneTimeSetUp]
        public void CleanDatabase()
        {
            DatabaseHelper.CleanEvents();
            DatabaseHelper.CleanEventStreams();
        }

        [Test]
        public async Task CreateStream_WillCreateStream_WhenStreamIdIsValid()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            // act
            await _sut
                .CreateStream(streamId)
                .ConfigureAwait(false);

            // assert
            var actual = await DatabaseHelper
                .GetLatestStreamCreated()
                .ConfigureAwait(false);

            actual
                .Should()
                .Be(streamId);
        }

        [Test]
        public void CreateStream_WillThrowException_WhenStreamIdIsInvalid()
        {
            // arrange
            var streamId = Guid.Empty;

            Func<Task> exceptionThrower = async () => await _sut
                .CreateStream(streamId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task CreateStream_WillThrowException_WhenStreamIdAlreadyExists()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            await DatabaseHelper
                .CreateEventStream(streamId)
                .ConfigureAwait(false);

            Func<Task> exceptionThrower = async () => await _sut
                .CreateStream(streamId)
                .ConfigureAwait(false);

            // act/assert
            exceptionThrower
                .ShouldThrow<PostgresException>();
        }

        [Test]
        public async Task CheckStream_WillReturnTrue_WhenStreamIdAlreadyExists()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            await DatabaseHelper
                .CreateEventStream(streamId)
                .ConfigureAwait(false);

            // act
            var streamExists = await _sut
                .CheckStream(streamId)
                .ConfigureAwait(false);

            // assert
            streamExists.Should().BeTrue();
        }

        [Test]
        public async Task CheckStream_WillReturnFalse_WhenStreamIdDoesNotExist()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            // act
            var streamExists = await _sut
                .CheckStream(streamId)
                .ConfigureAwait(false);

            // assert
            streamExists.Should().BeFalse();
        }

        [Test]
        public async Task GetAll_WillReturnAllEvents_WhenThereAreEventsForStream()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            await DatabaseHelper
                .CreateEventStream(streamId)
                .ConfigureAwait(false);

            var event1 = new StoredEvent("some type", DateTime.UtcNow, "{\"prop\":\"value\"}", 1L);
            var event2 = new StoredEvent("some type", DateTime.UtcNow, "{}", 2L);

            await DatabaseHelper
                .CreateEvent(streamId, event1.EventId, event1.TypeName, event1.OccurredOn, event1.EventBody)
                .ConfigureAwait(false);
            await DatabaseHelper
                .CreateEvent(streamId, event2.EventId, event2.TypeName, event2.OccurredOn, event2.EventBody)
                .ConfigureAwait(false);

            var expectedEvents = new List<StoredEvent>
            {
                event1,
                event2
            };

            // act
            var events = await _sut
                .GetAll(streamId)
                .ConfigureAwait(false);

            // assert
            events.ShouldAllBeEquivalentTo(expectedEvents);
        }
    }
}
