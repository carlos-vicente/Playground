using System;
using Playground.Messaging.Commands;
using Playground.TicketOffice.Domain.Write.Model;

namespace Playground.TicketOffice.Domain.Write.Commands
{
    public class CreateMovieTheaterCommand : DomainCommand<MovieTheater>
    {
        public string Name { get; set; }

        public int RoomsNumber { get; set; }

        public CreateMovieTheaterCommand(Guid aggregateRootId)
            : base(aggregateRootId) { }
    }
}