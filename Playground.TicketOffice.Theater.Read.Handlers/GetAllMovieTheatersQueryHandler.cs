using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.QueryService.Contracts;
using Playground.TicketOffice.Theater.Data.Contracts;
using Playground.TicketOffice.Theater.Domain;
using Playground.TicketOffice.Theater.Read.Queries;

namespace Playground.TicketOffice.Theater.Read.Handlers
{
    public class GetAllMovieTheatersQueryHandler 
        : IAsyncQueryHandler<GetAllMovieTheatersQuery, GetAllMovieTheatersQueryResult>
    {
        private readonly IMovieTheaterRepository _repository;

        public GetAllMovieTheatersQueryHandler(
            IMovieTheaterRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllMovieTheatersQueryResult> Handle(GetAllMovieTheatersQuery query)
        {
            var theaters = await _repository
                .GetAll()
                .ConfigureAwait(false);

            return new GetAllMovieTheatersQueryResult
            {
                Theaters = theaters ?? new List<MovieTheater>()
            };
        }
    }
}
