using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Playground.Tests;
using Playground.TicketOffice.Theater.Data.Contracts;
using Playground.TicketOffice.Theater.Domain;
using Playground.TicketOffice.Theater.Read.Queries;
using Ploeh.AutoFixture;

namespace Playground.TicketOffice.Theater.Read.Handlers.UnitTests
{
    public class GetAllMovieTheatersQueryHandlerTests
        : TestBaseWithSut<GetAllMovieTheatersQueryHandler>
    {
        [Test]
        public async Task Handle_WillReturnMappedMovieTheaters_WhenThereAreAvailable()
        {
            // arrange
            var theaters = Fixture
                .Build<MovieTheater>()
                .FromFactory<Guid, string>((id, name) => new MovieTheater(id, name))
                .CreateMany()
                .ToList();

            A.CallTo(() => Faker.Resolve<IMovieTheaterRepository>()
                .GetAll())
                .Returns(theaters);

            var expectedResult = new GetAllMovieTheatersQueryResult
            {
                Theaters = theaters
            };

            // act
            var result = await Sut
                .Handle(new GetAllMovieTheatersQuery())
                .ConfigureAwait(false);

            // assert
            result.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task Handle_WillReturnEmpty_WhenThereAreNoneAvailable()
        {
            // arrange
            var expectedResult = new GetAllMovieTheatersQueryResult
            {
                Theaters = new List<MovieTheater>()
            };

            // act
            var result = await Sut
                .Handle(new GetAllMovieTheatersQuery())
                .ConfigureAwait(false);

            // assert
            result.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task Handle_WillReturnEmpty_WhenThereNull()
        {
            // arrange
            A.CallTo(() => Faker.Resolve<IMovieTheaterRepository>()
                .GetAll())
                .Returns(null as IEnumerable<MovieTheater>);

            var expectedResult = new GetAllMovieTheatersQueryResult
            {
                Theaters = new List<MovieTheater>()
            };

            // act
            var result = await Sut
                .Handle(new GetAllMovieTheatersQuery())
                .ConfigureAwait(false);

            // assert
            result.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
