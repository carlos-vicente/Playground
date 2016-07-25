using System.Collections.Generic;
using System.Web.Mvc;
using Playground.TicketOffice.Web.Management.Models;

namespace Playground.TicketOffice.Web.Management.Controllers
{
    public class HomeController : Controller
    {
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