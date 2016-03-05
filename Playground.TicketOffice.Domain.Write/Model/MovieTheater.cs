using System;
using System.Collections.Generic;
using Playground.Domain;
using Playground.Domain.Events;
using Playground.Domain.Model;
using Playground.TicketOffice.Domain.Write.Events;

namespace Playground.TicketOffice.Domain.Write.Model
{
    public class MovieTheater 
        : AggregateRoot,
        IEmit<MovieTheaterCreated>
    {
        public string Name { get; set; }

        public IEnumerable<TheaterRoom> Rooms { get; set; }

        public MovieTheater(Guid id) : base(id) { }

        public void CreateMovieTheater(string name)
        {
            var @event = new MovieTheaterCreated
            {
                Metadata = new Metadata(Id),
                Name = name
            };

            Events.Add(@event);
        }

        #region Events Apply

        void IEmit<MovieTheaterCreated>.Apply(MovieTheaterCreated e)
        {
            Name = e.Name;
            Rooms = new List<TheaterRoom>();
        }

        #endregion
    }
}