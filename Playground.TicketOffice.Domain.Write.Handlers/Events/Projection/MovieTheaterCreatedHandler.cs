//using System.Threading.Tasks;
//using Playground.Domain.Persistence;
//using Playground.TicketOffice.Domain.Write.Events;
//using Playground.TicketOffice.Domain.Write.Model;
//using Playground.TicketOffice.Read.Data.Contracts;
//using Rebus.Handlers;

//namespace Playground.TicketOffice.Domain.Write.Handlers.Events.Projection
//{
//    public class MovieTheaterCreatedHandler
//        : IHandleMessages<MovieTheaterCreated>
//    {
//        private readonly IAggregateContext _aggregateContext;
//        private readonly IMovieTeatherRepository _movieTeatherRepository;

//        public MovieTheaterCreatedHandler(
//            IAggregateContext aggregateContext,
//            IMovieTeatherRepository movieTeatherRepository)
//        {
//            _aggregateContext = aggregateContext;
//            _movieTeatherRepository = movieTeatherRepository;
//        }

//        public async Task Handle(MovieTheaterCreated @event)
//        {
//            var aggregate = await _aggregateContext
//                .Load<MovieTheater>(@event.Metadata.AggregateRootId)
//                .ConfigureAwait(false);
//        }
//    }
//}