using System.Collections.Generic;
using System.Web.Mvc;
using Playground.TicketOffice.Web.Models;

namespace Playground.TicketOffice.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var theaters = new List<Theater>
            {
                new Theater { Name = "Colombo" },
                new Theater { Name = "Odivelas" }
            };

            return View(theaters);
        }
    }
}