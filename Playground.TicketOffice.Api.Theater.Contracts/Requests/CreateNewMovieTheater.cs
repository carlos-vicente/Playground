﻿using Playground.Api.Contracts;

namespace Playground.TicketOffice.Api.Theater.Contracts.Requests
{
    public class CreateNewMovieTheater : IRequest
    {
        public string Name { get; set; }

        public string GetRelativeUrl()
        {
            return Routes.CreateNewMovieTheater;
        }
    }
}
