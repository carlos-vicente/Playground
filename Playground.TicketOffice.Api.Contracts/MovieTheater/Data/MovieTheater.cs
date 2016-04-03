using System;

namespace Playground.TicketOffice.Api.Contracts.MovieTheater.Data
{
    public class MovieTheater
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int RoomsNumber { get; set; } 
    }
}