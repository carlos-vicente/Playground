using System.Collections.Generic;
using Playground.TicketOffice.Theater.Domain;

namespace Playground.TicketOffice.Theater.Read.Queries
{
    public class GetAllMovieTheatersQueryResult
    {
        public IEnumerable<MovieTheater> Theaters { get; set; }
    }
}