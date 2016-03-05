using System;
using System.Collections.Generic;
using Playground.Domain;
using Playground.Domain.Model;

namespace Playground.TicketOffice.Domain.Write.Model
{
    public class TheaterRoom : Entity
    {
        public IEnumerable<Seat> Seats { get; set; }
        
        public TheaterRoom(Guid id) : base(id)
        {
        }
    }
}