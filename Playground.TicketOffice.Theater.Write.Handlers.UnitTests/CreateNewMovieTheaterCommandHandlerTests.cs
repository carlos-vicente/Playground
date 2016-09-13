using System;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Playground.Tests;
using Playground.TicketOffice.Theater.Data.Contracts;
using Playground.TicketOffice.Theater.Domain;
using Playground.TicketOffice.Theater.Write.Messages;
using Ploeh.AutoFixture;

namespace Playground.TicketOffice.Theater.Write.Handlers.UnitTests
{
    
    public class CreateNewMovieTheaterCommandHandlerTests 
        : TestBaseWithSut<CreateNewMovieTheaterCommandHandler>
    {
        [Test]
        public async Task Handle_WillCreateNewMovie()
        {
            // arrange
            var command = Fixture
                .Create<CreateNewMovieTheaterCommand>();

            // act
            await Sut.Handle(command)
                .ConfigureAwait(false);

            // arrange
            A.CallTo(() => Faker.Resolve<IMovieTheaterRepository>()
                .Create(A<MovieTheater>.That.Matches(mt =>
                    mt.Name.Equals(command.Name)
                    && mt.Id != Guid.Empty)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
