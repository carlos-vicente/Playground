using System;
using System.Collections.Generic;
using System.Web.Http;
using Playground.TicketOffice.Api.Theater.Contracts;
using Playground.TicketOffice.Api.Theater.Contracts.Data;

namespace Playground.TicketOffice.Api.Theater.Controllers
{
    [RoutePrefix("theater")]
    public class TheaterController : ApiController
    {
        [Route("")]
        [HttpGet]
        public GetAllResult GetAll()
        {
            // execute query, which returns model
            // map model to api data
            // build result object with api data

            return new GetAllResult
            {
                Theaters = new List<MovieTheater>
                {
                    new MovieTheater { Id = Guid.NewGuid(), Name = "Colombo" },
                    new MovieTheater { Id = Guid.NewGuid(), Name = "Odivelas" }
                }
            };
        }
    }
}