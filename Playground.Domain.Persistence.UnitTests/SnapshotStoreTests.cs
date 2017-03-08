using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Core.Serialization;
using Playground.Domain.Model;
using Playground.Domain.Persistence.Snapshots;
using Playground.Domain.Persistence.UnitTests.TestModel;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.UnitTests
{
    public class SnapshotStoreTests : SimpleTestBase
    {
        [Test]
        public void GetLastestSnaptshot_WillThrowException_WhenStreamIdIsInvalid()
        {
            // arrange
            var invalidStreamId = default(Guid);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .GetLastestSnaptshot<TestAggregateState>(invalidStreamId)
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Be("streamId");
        }

        [Test]
        public async Task GetLastestSnaptshot_WillReturnNull_WhenThereIsNoSnapshotAvailable()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            var sut = Faker.Resolve<SnapshotStore>();

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshot(streamId))
                .Returns(null as StoredSnapshot);

            // act
            var actual = await sut
                .GetLastestSnaptshot<TestAggregateState>(streamId)
                .ConfigureAwait(false);

            // assert
            actual
                .Should()
                .BeNull();
        }

        [Test]
        public async Task GetLastestSnaptshot_WillReturnLatestSnapshot_WhenItIsAvailable()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            var sut = Faker.Resolve<SnapshotStore>();

            var storedSnapshot = new StoredSnapshot(
                Fixture.Create<long>(),
                Fixture.Create<DateTime>(),
                Fixture.Create<string>());

            var snapshotState = Fixture.Create<TestAggregateState>();

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshot(streamId))
                .Returns(storedSnapshot);

            A.CallTo(() => Faker.Resolve<IObjectSerializer>()
                    .Deserialize<TestAggregateState>(storedSnapshot.Data))
                .Returns(snapshotState);

            var expected = new Snapshot<TestAggregateState>(
                storedSnapshot.Version,
                storedSnapshot.TakenOn,
                snapshotState);

            // act
            var actual = await sut
                .GetLastestSnaptshot<TestAggregateState>(streamId)
                .ConfigureAwait(false);

            // assert
            actual
                .ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenStreamIdIsInvalid()
        {
            // arrange
            var invalidStreamId = default(Guid);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .StoreNewSnapshot(invalidStreamId, Fixture.Create<Snapshot<TestAggregateState>>())
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Be("streamId");
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenSnapshotIsInvalid()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .StoreNewSnapshot<TestAggregateState>(streamId, null)
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Be("snapshot");
        }

        [TestCase(0L)]
        [TestCase(-34L)]
        public void StoreNewSnapshot_WillThrowException_WhenSnapshotVersionIsInvalid(
            long invalidVersion)
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                invalidVersion,
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                Fixture.Create<TestAggregateState>());

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .Message
                .Should()
                .Contain("version");
        }

        public static DateTime[] invalidTimestampCases = new[]
        {
            DateTime.MaxValue,
            DateTime.UtcNow.AddHours(3)
        };

        [TestCaseSource("invalidTimestampCases")]
        public void StoreNewSnapshot_WillThrowException_WhenSnapshotTakenTimestampIsInvalid(
            DateTime invalidTimestamp)
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                invalidTimestamp,
                Fixture.Create<TestAggregateState>());

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .Message
                .Should()
                .Contain("taken on");
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenSnapshotDataIsInvalid()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                null);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> expcetionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            expcetionThrower
                .ShouldThrow<ArgumentException>()
                .And
                .Message
                .Should()
                .Contain("data");
        }

        [Test]
        public async Task StoreNewSnapshot_WillSaveNewSnapshot_WhenSnapshotIsValid()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                Fixture.Create<TestAggregateState>());

            var serialized = Fixture.Create<string>();

            A.CallTo(() => Faker.Resolve<IObjectSerializer>()
                    .Serialize(snapshot.Data))
                .Returns(serialized);

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshotVersion(streamId))
                .Returns(snapshot.Version - 1);

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .SaveSnapshot(streamId, A<StoredSnapshot>._))
                .Returns(true);

            var sut = Faker.Resolve<SnapshotStore>();

            // act
            await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .SaveSnapshot(
                        streamId,
                        A<StoredSnapshot>
                            .That
                            .Matches(ss => ss.Version == snapshot.Version
                                           && ss.TakenOn == snapshot.TakenOn
                                           && ss.Data == serialized)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenThereIsAnotherSnapshotWithTheSameVersion()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                Fixture.Create<TestAggregateState>());

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshotVersion(streamId))
                .Returns(snapshot.Version);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> exceptionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenThereIsAnotherSnapshotWithAnHigherVersion()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                Fixture.Create<TestAggregateState>());
            
            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshotVersion(streamId))
                .Returns(snapshot.Version + 1);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> exceptionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void StoreNewSnapshot_WillThrowException_WhenSaveFails()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var snapshot = new Snapshot<TestAggregateState>(
                Fixture.Create<long>(),
                DateTime.UtcNow.Subtract(Fixture.Create<TimeSpan>()),
                Fixture.Create<TestAggregateState>());

            var serialized = Fixture.Create<string>();

            A.CallTo(() => Faker.Resolve<IObjectSerializer>()
                    .Serialize(snapshot.Data))
                .Returns(serialized);

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .GetLatestSnapshotVersion(streamId))
                .Returns(snapshot.Version - 1);

            A.CallTo(() => Faker.Resolve<ISnapshotRepository>()
                    .SaveSnapshot(streamId, A<StoredSnapshot>._))
                .Returns(false);

            var sut = Faker.Resolve<SnapshotStore>();

            Func<Task> exceptionThrower = async () => await sut
                .StoreNewSnapshot(streamId, snapshot)
                .ConfigureAwait(false);

            // act & assert
            exceptionThrower
                .ShouldThrow<InvalidOperationException>();
        }
    }
}