using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators.Reflection;
using NUnit.Framework;
using Playground.Tests;
using Playground.TicketOffice.Domain.Write.Commands;
using Playground.TicketOffice.Domain.Write.Handlers.Commands;
using Playground.TicketOffice.Domain.Write.Model;
using Ploeh.AutoFixture;

namespace Playground.TicketOffice.Domain.Write.Handlers.UnitTests.Commands
{
    public class CreateMovieTheaterCommandHandlerTests 
        : TestBaseWithSut<CreateMovieTheaterCommandHandler>
    {
        [Test]
        public async Task Handle_WillDoStuff()
        {
            // arrange
            var movieTheater = Faker.Resolve<MovieTheater>(new NamedParameter("id", Guid.NewGuid()));
            var command = Fixture.Create<CreateMovieTheaterCommand>();

            // act
            await Sut
                .Handle(command)
                .ConfigureAwait(false);
        }
    }
}
