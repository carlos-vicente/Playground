//using System.Threading.Tasks;
//using Playground.Domain.Persistence;
//using Playground.Messaging.Persistence;
//using Playground.TicketOffice.Domain.Write.Commands;
//using Playground.TicketOffice.Domain.Write.Model;
//using Rebus.Handlers;

//namespace Playground.TicketOffice.Domain.Write.Handlers.Commands
//{
//    public class CreateMovieTheaterCommandHandler 
//        : AsyncDomainCommandHandler<CreateMovieTheaterCommand, MovieTheater>,
//        IHandleMessages<CreateMovieTheaterCommand>
//    {
//        public CreateMovieTheaterCommandHandler(IAggregateContext aggregateContext)
//            : base(aggregateContext)
//        {
//        }

//        protected override Task HandleOnAggregate(
//            CreateMovieTheaterCommand command,
//            MovieTheater aggregate)
//        {
//            aggregate
//                .CreateMovieTheater(command.Name, command.RoomsNumber);

//            return Task.FromResult(1);
//        }
//    }
//}