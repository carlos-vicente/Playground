using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
    /// 
    /// </summary>
    [RoutePrefix("theater")]
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
        /// <returns>A collection of movie theaters</returns>
        [Route("")]
        [HttpGet]
        public async Task<GetAllResult> GetAll()
        {
            var query = new GetAllMovieTheatersQuery();

            var theatersResult = await _queryService
                .QueryAsync<GetAllMovieTheatersQuery, GetAllMovieTheatersQueryResult>(query)
                .ConfigureAwait(false);

            return new GetAllResult
            {
                Theaters = theatersResult
                    .Theaters
                    .Select(mt => new MovieTheater
                    {
                        Id = mt.Id,
                        Name = mt.Name
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Creates a new movie theater
        /// </summary>
        /// <param name="createRequest">The new movie theater's information</param>
        /// <response code="201">The new movie theater was created</response>
        /// <response code="500">An error occurred while creating the movie theater</response>
        [Route("")]
        [HttpPost]
        public async Task Create(CreateNewMovieTheater createRequest)
        {
            await _messageBus
                .SendCommand(new CreateNewMovieTheaterCommand
                {
                    Name = createRequest.Name
                })
                .ConfigureAwait(false);
        }
    }
}