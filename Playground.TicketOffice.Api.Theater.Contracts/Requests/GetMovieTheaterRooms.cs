using System;
using Playground.Api.Contracts;

namespace Playground.TicketOffice.Api.Theater.Contracts.Requests
{
    public class GetMovieTheaterRooms : IRequest
    {
        public Guid TheaterId { get; set; }

        public string GetRelativeUrl()
        {
            return Routes
                .GetMovieTheaterRooms
                .Replace("{theaterId:guid}", TheaterId.ToString());
        }
    }
}