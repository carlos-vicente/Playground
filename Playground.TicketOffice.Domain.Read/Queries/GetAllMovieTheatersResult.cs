using System.Collections.Generic;
using Playground.TicketOffice.Domain.Read.Model;

namespace Playground.TicketOffice.Domain.Read.Queries
{
    public class GetAllMovieTheatersResult
    {
        public IEnumerable<MovieTheater> Type { get; set; }
    }
}