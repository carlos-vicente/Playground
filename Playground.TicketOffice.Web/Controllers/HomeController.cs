using System.Collections.Generic;
using System.Web.Mvc;
using Playground.Http;
using Playground.TicketOffice.Web.Models;
using RestSharp;

namespace Playground.TicketOffice.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClient _client;

        public HomeController(IHttpClient client)
        {
            _client = client;
        }

        // GET: Home
        public ActionResult Index()
        {
            _client.Get<>()

            var theaters = new List<Theater>
            {
                new Theater { Name = "Colombo" },
                new Theater { Name = "Odivelas" }
            };

            return View(theaters);
        }
    }
}