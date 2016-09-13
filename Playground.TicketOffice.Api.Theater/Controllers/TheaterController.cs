using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Playground.Messaging;
using Playground.QueryService.Contracts;
using Playground.TicketOffice.Api.Theater.Contracts.Data;
using Playground.TicketOffice.Api.Theater.Contracts.Requests;
using Playground.TicketOffice.Api.Theater.Contracts.Responses;
using Playground.TicketOffice.Theater.Read.Queries;
using Playground.TicketOffice.Theater.Write.Messages;

namespace Playground.TicketOffice.Api.Theater.Controllers
{
    /// <summary>
    /// The controller to manage the movie theater resource
    /// </summary>
    [RoutePrefix("theaters")]
    public class TheaterController : ApiController
    {
        private readonly IMessageBus _messageBus;
        private readonly IQueryService _queryService;

        /// <summary>
        /// Theater controller constructor. Should only be used by IoC container and tests.
        /// </summary>
        /// <param name="messageBus">The message bus dependency, used to send commands</param>
        /// <param name="queryService">The query service dependency, used to obtain data</param>
        public TheaterController(
            IMessageBus messageBus,
            IQueryService queryService)
        {
            _messageBus = messageBus;
            _queryService = queryService;
        }

        /// <summary>
        /// Gets all movie theaters available
        /// </summary>
        /// <returns >A collection of movie theaters</returns>
        /// <response code="200">The request was processed correctly, the data will be in the response body</response>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(GetAllResult))]
        public async Task<IHttpActionResult> GetAll()
        {
            var query = new GetAllMovieTheatersQuery();

            var theatersResult = await _queryService
                .QueryAsync<GetAllMovieTheatersQuery, GetAllMovieTheatersQueryResult>(query)
                .ConfigureAwait(false);

            return Ok(new GetAllResult
            {
                Theaters = theatersResult
                    .Theaters
                    .Select(mt => new MovieTheater
                    {
                        Id = mt.Id,
                        Name = mt.Name
                    })
                    .ToList()
            });
        }

        /// <summary>
        /// Creates a new movie theater
        /// </summary>
        /// <param name="createRequest">The new movie theater's information</param>
        /// <response code="200">The request was processed correctly</response>
        /// <response code="500">An error occurred while creating the movie theater</response>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Create(CreateNewMovieTheater createRequest)
        {
            await _messageBus
                .SendCommand(new CreateNewMovieTheaterCommand
                {
                    Name = createRequest.Name
                })
                .ConfigureAwait(false);

            return Ok();
        }

        /// <summary>
        /// Gets a movie theater that matches a given identifier
        /// </summary>
        /// <param name="theaterId">The identifier to search for</param>
        /// <returns></returns>
        /// <response code="200">The request was processed correctly, the data will be in the response body</response>
        [Route("{theaterId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetById(Guid theaterId)
        {
            return Ok(theaterId);
        }

        /// <summary>
        /// Gets all movies currently showing at the theater
        /// </summary>
        /// <param name="theaterId">The theater's identifier</param>
        /// <returns></returns>
        /// <response code="200">The request was processed correctly, the data will be in the response body</response>
        [Route("{theaterId}/movies")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTheaterMovies(Guid theaterId)
        {
            return Ok(theaterId);
        }

        /// <summary>
        /// Gets all rooms available at the theater
        /// </summary>
        /// <param name="theaterId">The theater's identifier</param>
        /// <returns></returns>
        /// <response code="200">The request was processed correctly, the data will be in the response body</response>
        [Route("{theaterId}/rooms")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTheaterRooms(Guid theaterId)
        {
            return Ok(theaterId);
        }
    }
}