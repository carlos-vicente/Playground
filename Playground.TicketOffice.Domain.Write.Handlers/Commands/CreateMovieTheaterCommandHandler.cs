using System.Threading.Tasks;
using Playground.Domain.Persistence;
using Playground.Messaging.Domain;
using Playground.TicketOffice.Domain.Write.Commands;
using Playground.TicketOffice.Domain.Write.Model;

namespace Playground.TicketOffice.Domain.Write.Handlers.Commands
{
    public class CreateMovieTheaterCommandHandler 
        : AsyncDomainCommandHandler<CreateMovieTheaterCommand, MovieTheater>
    {
        public CreateMovieTheaterCommandHandler(IAggregateContext aggregateContext)
            : base(aggregateContext)
        {
        }

        protected override Task HandleOnAggregate(
            CreateMovieTheaterCommand command,
            MovieTheater aggregate)
        {
            aggregate.CreateMovieTheater(command.Name);

            return Task.FromResult(1);
        }
    }
}