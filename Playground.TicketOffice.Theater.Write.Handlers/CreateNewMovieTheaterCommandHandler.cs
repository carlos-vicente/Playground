using System;
using System.Threading.Tasks;
using Playground.Messaging.Commands;
using Playground.TicketOffice.Theater.Data.Contracts;
using Playground.TicketOffice.Theater.Domain;
using Playground.TicketOffice.Theater.Write.Messages;
using Rebus.Handlers;

namespace Playground.TicketOffice.Theater.Write.Handlers
{
    public class CreateNewMovieTheaterCommandHandler
        : IAsyncCommandHandler<CreateNewMovieTheaterCommand>,
        IHandleMessages<CreateNewMovieTheaterCommand>
    {
        private readonly IMovieTheaterRepository _movieTheaterRepository;

        public CreateNewMovieTheaterCommandHandler(
            IMovieTheaterRepository movieTheaterRepository)
        {
            _movieTheaterRepository = movieTheaterRepository;
        }

        public async Task Handle(CreateNewMovieTheaterCommand command)
        {
            var movieTheaterToCreate = new MovieTheater(command.Name);
            
            await _movieTheaterRepository
                .Create(movieTheaterToCreate)
                .ConfigureAwait(false);
        }
    }
}
