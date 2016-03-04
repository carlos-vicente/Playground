using Playground.Domain;

namespace Playground.TicketOffice.Domain.Write.Events
{
    public class MovieTheaterCreated : DomainEvent
    {
        public string Name { get; set; }
    }
}