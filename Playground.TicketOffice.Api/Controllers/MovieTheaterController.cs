using System;
using System.Collections.Generic;
using System.Web.Http;
using Playground.TicketOffice.Api.Contracts;
using Playground.TicketOffice.Api.Contracts.MovieTheater;
using Playground.TicketOffice.Api.Contracts.MovieTheater.Data;

namespace Playground.TicketOffice.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for showing movie theater's information
    /// </summary>
    [RoutePrefix("theater")]
    public class MovieTheaterController : ApiController
    {
        /// <summary>
        /// Gets all the available movie theaters
        /// </summary>
        /// <returns>Complete list of movie theaters</returns>
        [Route("")]
        [HttpGet]
        public GetAllResult GetAll()
        {
            // execute query, which returns read model
            // map read model to api data
            // build result object with api data

            return new GetAllResult
            {
                Theaters = new List<MovieTheater>
                {
                    new MovieTheater
                    {
                        Id = Guid.NewGuid(),
                        Name = "Colombo",
                        RoomsNumber = 10
                    },
                    new MovieTheater
                    {
                        Id = Guid.NewGuid(),
                        Name = "Odivelas",
                        RoomsNumber = 7
                    }
                }
            };
        }

        /// <summary>
        /// Gets the movie theaters with the specified identifier
        /// <param name="id">The theater's identifier</param>
        /// </summary>
        /// <returns>The specified movie theater (if it exists)</returns>
        [Route("{id}")]
        [HttpGet]
        public GetResult Get(Guid id)
        {
            return new GetResult
            {
                Theater = new MovieTheater
                {
                    Id = id,
                    Name = "um qualquer",
                    RoomsNumber = 10
                }
            };
        }

        [Route("")]
        [HttpPost]
        public void Create(CreateRequest request)
        {
            // send a command to create this movie theater
        }
    }
}
