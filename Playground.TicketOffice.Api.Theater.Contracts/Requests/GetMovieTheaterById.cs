using System;
using Playground.Api.Contracts;

namespace Playground.TicketOffice.Api.Theater.Contracts.Requests
{
    public class GetMovieTheaterById : IRequest
    {
        public Guid TheaterId { get; set; }

        public string GetRelativeUrl()
        {
            return Routes
                .GetMovieTheaterById
                .Replace("{theaterId:guid}", TheaterId.ToString());
        }
    }
}