using Playground.Api.Contracts;

namespace Playground.TicketOffice.Api.Theater.Contracts.Requests
{
    public class GetAllMovieTheaters : IRequest
    {
        public string GetRelativeUrl()
        {
            return Routes.GetAllMovieTheaters;
        }
    }
}