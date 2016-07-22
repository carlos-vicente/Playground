using System.Collections.Generic;
using Playground.TicketOffice.Api.Theater.Contracts.Data;

namespace Playground.TicketOffice.Api.Theater.Contracts
{
    public class GetAllResult
    {
        public IEnumerable<MovieTheater> Theaters { get; set; } 
    }
}