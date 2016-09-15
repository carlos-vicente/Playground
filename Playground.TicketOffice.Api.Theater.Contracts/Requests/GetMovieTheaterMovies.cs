using System;
using Playground.Api.Contracts;

namespace Playground.TicketOffice.Api.Theater.Contracts.Requests
{
    public class GetMovieTheaterMovies : IRequest
    {
        public Guid TheaterId { get; set; }

        public string GetRelativeUrl()
        {
            return Routes
                .GetMovieTheaterMovies
                .Replace("{theaterId:guid}", TheaterId.ToString());
        }
    }
}