using System;

namespace Playground.TicketOffice.Theater.Domain
{
    public class MovieTheater
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public MovieTheater(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public MovieTheater(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
