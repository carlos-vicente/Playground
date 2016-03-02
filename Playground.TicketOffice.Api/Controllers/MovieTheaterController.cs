using System;
using System.Collections.Generic;
using System.Web.Http;

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
        public IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Gets the movie theaters with the specified identifier
        /// <param name="id">The theater's identifier</param>
        /// </summary>
        /// <returns>The specified movie theater (if it exists)</returns>
        [Route("{id}")]
        [HttpGet]
        public object Get(Guid id)
        {
            return new
            {
                Id = id,
                Name = "something"
            };
        }
    }
}
