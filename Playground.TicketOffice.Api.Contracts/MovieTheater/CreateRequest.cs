using System.Runtime.CompilerServices;

namespace Playground.TicketOffice.Api.Contracts.MovieTheater
{
    public class CreateRequest
    {
        public Data.MovieTheater TheaterToCreate { get; set; }
    }
}