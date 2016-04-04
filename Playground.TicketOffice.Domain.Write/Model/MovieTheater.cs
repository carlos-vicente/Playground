using System;
using System.Collections.Generic;
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

        public void CreateMovieTheater(string name, int roomsNumber)
        {
            var @event = new MovieTheaterCreated(Id)
            {
                Name = name,
                RoomsNumber = roomsNumber
            };

            When(@event);
        }

        #region Events Apply

        void IEmit<MovieTheaterCreated>.Apply(MovieTheaterCreated e)
        {
            Name = e.Name;
            Rooms = new TheaterRoom[e.RoomsNumber];
        }

        #endregion
    }
}