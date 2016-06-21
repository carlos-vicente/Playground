using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Playground.Tests;
using Playground.TicketOffice.Domain.Write.Model;
using Ploeh.AutoFixture;

namespace Playground.TicketOffice.Domain.Write.UnitTests
{
    public class MovieTheaterTests : SimpleTestBase
    {
        private MovieTheater _sut;

        public override void SetUp()
        {
            base.SetUp();

            _sut = new MovieTheater(Fixture.Create<Guid>());
        }

        [Test]
        public void CreateMovieTheater_WillApplyMovieTheaterCreated()
        {
            // arrange
            var name = Fixture.Create<string>();
            var numOfRooms = Fixture.Create<int>();

            // act
            _sut.CreateMovieTheater(name, numOfRooms);

            // assert
            _sut.Name.ShouldBeEquivalentTo(name);
            _sut.Rooms.Count().ShouldBeEquivalentTo(numOfRooms);
        }
    }
}