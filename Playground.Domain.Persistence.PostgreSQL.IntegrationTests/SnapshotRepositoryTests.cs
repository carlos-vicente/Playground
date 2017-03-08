using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Playground.Core;
using Playground.Domain.Persistence.PostgreSQL.TestsHelper;
using Playground.Domain.Persistence.Snapshots;
using Playground.Tests;
using Ploeh.AutoFixture;

namespace Playground.Domain.Persistence.PostgreSQL.IntegrationTests
{
    public class SnapshotRepositoryTests : SimpleTestBase
    {
        private SnapshotRepository _sut;

        public override void SetUp()
        {
            base.SetUp();

            DatabaseHelper.CleanEventStreams();

            _sut = new SnapshotRepository(DatabaseHelper.GetConnectionStringBuilder());
        }

        [Test]
        public async Task GetLatestSnapshot_WillGetLatestSnapshot_WhenMultipleAreAvailable()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var streamName = Fixture.Create<string>();

            var now = DateTime.UtcNow.GetToTheMillisecond();

            await DatabaseHelper
                .CreateEventStream(streamId, streamName)
                .ConfigureAwait(false);

            var snapshot1 = new StoredSnapshot(1L, now.AddHours(-2), "{\"prop\":\"value\"}");
            var snapshot2 = new StoredSnapshot(10L, now, "{\"prop\":\"value2\"}");

            await DatabaseHelper
                .CreateSnapshot(streamId, snapshot1)
                .ConfigureAwait(false);
            await DatabaseHelper
                .CreateSnapshot(streamId, snapshot2)
                .ConfigureAwait(false);

            var expected = snapshot2;

            // act
            var actual = await _sut
                .GetLatestSnapshot(streamId)
                .ConfigureAwait(false);

            // assert
            actual
                .ShouldBeEquivalentTo(expected);
        }

        [Test]
        public async Task GetLatestSnapshot_WillReturnNull_WhenNoSnapshotsAreAvailable()
        {
            // arrange
            var streamId = Fixture.Create<Guid>();
            var streamName = Fixture.Create<string>();

            await DatabaseHelper
                .CreateEventStream(streamId, streamName)
                .ConfigureAwait(false);

            // act
            var actual = await _sut
                .GetLatestSnapshot(streamId)
                .ConfigureAwait(false);

            // assert
            actual
                .Should()
                .BeNull();
        }

        [Test]
        public void GetLatestSnapshot_ThrowsArgumentException_WhenStreamIdIsInvalid()
        {
            // arrange
            var streamId = default(Guid);

            Func<Task> exceptionThrower = async () => await _sut
                .GetLatestSnapshot(streamId)
                .ConfigureAwait(false);

            // act & assert
            exceptionThrower
                .ShouldThrow<ArgumentException>();
        }
    }
}