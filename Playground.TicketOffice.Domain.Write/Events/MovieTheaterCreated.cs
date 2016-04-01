using System;
using Playground.Domain;
using Playground.Domain.Events;

namespace Playground.TicketOffice.Domain.Write.Events
{
    public class MovieTheaterCreated : DomainEvent
    {
        public string Name { get; set; }

        public MovieTheaterCreated(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }
}