using System.Collections.Generic;
using Playground.TicketOffice.Api.Theater.Contracts.Data;

namespace Playground.TicketOffice.Api.Theater.Contracts.Responses
{
    public class GetAllMovieTheatersResult
    {
        public IEnumerable<MovieTheater> Theaters { get; set; } 
    }
}