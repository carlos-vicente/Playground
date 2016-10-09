using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Playground.Http;
using Playground.TicketOffice.Api.Theater.Contracts.Requests;
using Playground.TicketOffice.Api.Theater.Contracts.Responses;
using Playground.TicketOffice.Web.Configuration;
using Playground.TicketOffice.Web.Models;

namespace Playground.TicketOffice.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClient _client;
        private readonly IEndpointsConfiguration _endpointsConfiguration;

        public HomeController(
            IHttpClient client,
            IEndpointsConfiguration endpointsConfiguration)
        {
            _client = client;
            _endpointsConfiguration = endpointsConfiguration;
        }

        // GET: Home
        public async Task<ActionResult> Index()
        {
            var result = await _client
                .Get<GetAllMovieTheaters, GetAllMovieTheatersResult>(
                    _endpointsConfiguration.MovieTheaterEndpoint,
                    new GetAllMovieTheaters())
                .ConfigureAwait(false);

            var theaters = result
                .Theaters
                .Select(t => new Theater
                {
                    Name = t.Name
                })
                .ToList();

            return View(theaters);
        }
    }
}