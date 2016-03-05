using Playground.Domain;
using Playground.Domain.Model;

namespace Playground.TicketOffice.Domain.Write.Model
{
    public class Seat : ValueObject
    {
        public int X { get; set; }

        public int Y { get; set; }

        public bool IsAvailable { get; set; }
    }
}
