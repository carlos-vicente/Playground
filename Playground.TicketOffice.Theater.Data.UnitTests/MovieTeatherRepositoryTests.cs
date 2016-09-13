using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Data.Contracts;
using Playground.Tests;
using Playground.TicketOffice.Theater.Data;
using Playground.TicketOffice.Theater.Domain;
using Ploeh.AutoFixture;

namespace Playground.TicketOffice.Theater.Read.Data.UnitTests
{
    public class MovieTheaterRepositoryTests 
        : TestBaseWithSut<MovieTheaterRepository>
    {
        [Test]
        public async Task Create_WillExecuteCommand_WhenMovieTheaterIsValid()
        {
            // arrange
            var movieTheater = Fixture
                .Build<MovieTheater>()
                .FromFactory<Guid, string>((id, name) => new MovieTheater(id, name))
                .Create();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

                // act
            await Sut.Create(movieTheater).ConfigureAwait(false);

            // assert
            A.CallTo(() => Faker.Resolve<IConnection>()
                .ExecuteCommand(A<string>._, A<object>.That.IsSameAs(movieTheater)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Create_WillThrowException_WhenMovieTheaterIsNull()
        {
            // arrange
            
            // act
            Func<Task> exThrower = async () => await Sut.Create(null).ConfigureAwait(false);

            // assert
            exThrower
                .ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public async Task GetAll_WillReturnAllAvailable_WhenThereAreAvailable()
        {
            // arrange
            var theaters = Fixture
                .Build<MovieTheater>()
                .FromFactory<Guid, string>((id, name) => new MovieTheater(id, name))
                .CreateMany()
                .ToList();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

            A.CallTo(() => Faker.Resolve<IConnection>()
                .ExecuteQueryMultiple<dynamic>(A<string>._, A<object>._))
                .Returns(theaters);

            // act
            var result = await Sut.GetAll().ConfigureAwait(false);

            // assert
            result.ShouldAllBeEquivalentTo(theaters);
        }

        [Test]
        public async Task GetAll_WillReturnEmpty_WhenThereAreNoAvailable()
        {
            // arrange
            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

            // act
            var result = await Sut.GetAll().ConfigureAwait(false);

            // assert
            result
                .Should()
                .BeEmpty();
        }

        [Test]
        public async Task GetAll_WillReturnEmpty_WhenIsNull()
        {
            // arrange
            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

            A.CallTo(() => Faker.Resolve<IConnection>()
                .ExecuteQueryMultiple<dynamic>(A<string>._, A<object>._))
                .Returns(null as IEnumerable<dynamic>);

            // act
            var result = await Sut.GetAll().ConfigureAwait(false);

            // assert
            result
                .Should()
                .BeEmpty();
        }

        [Test]
        public async Task GetById_WillReturnSpecified_WhenItExists()
        {
            // arrange
            var theater = Fixture
                .Build<MovieTheater>()
                .FromFactory<Guid, string>((id, name) => new MovieTheater(id, name))
                .Create();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

            A.CallTo(() => Faker.Resolve<IConnection>()
                .ExecuteQuerySingle<dynamic>(A<string>._, A<object>._))
                .Returns(theater);

            // act
            var result = await Sut.GetById(theater.Id).ConfigureAwait(false);

            // assert
            result.ShouldBeEquivalentTo(theater);
        }

        [Test]
        public async Task GetById_WillReturnsNull_WhenItDoesNotExist()
        {
            // arrange
            var nonExistingId = Fixture.Create<Guid>();

            A.CallTo(() => Faker.Resolve<IConnectionFactory>()
                .CreateConnection())
                .Returns(Faker.Resolve<IConnection>());

            A.CallTo(() => Faker.Resolve<IConnection>()
                .ExecuteQuerySingle<dynamic>(A<string>._, A<object>._))
                .Returns(null as object);

            // act
            var result = await Sut.GetById(nonExistingId).ConfigureAwait(false);

            // assert
            result.Should().BeNull();
        }
    }
}
