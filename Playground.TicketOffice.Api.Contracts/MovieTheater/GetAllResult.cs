using System.Collections.Generic;

namespace Playground.TicketOffice.Api.Contracts.MovieTheater
{
    public class GetAllResult
    {
        public IEnumerable<Data.MovieTheater> Theaters { get; set; } 
    }
}