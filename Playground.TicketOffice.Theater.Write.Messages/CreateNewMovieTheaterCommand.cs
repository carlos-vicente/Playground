using Playground.Messaging.Commands;

namespace Playground.TicketOffice.Theater.Write.Messages
{
    public class CreateNewMovieTheaterCommand : ICommand
    {
        public string Name { get; set; }
    }
}
