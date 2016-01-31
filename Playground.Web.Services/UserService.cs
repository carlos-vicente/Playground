using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.QueryService.Contracts;
using Playground.Web.Services.Contracts;
using Playground.Web.Services.Contracts.Entities;
using Playground.Web.Services.Queries;

namespace Playground.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IQueryService _queryService;

        public UserService(IQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<Profile> GetProfile(string userId)
        {
            var query = new GetProfileQuery
            {
                UserId = userId
            };

            var result = await _queryService
                .QueryAsync<GetProfileQueryResult, GetProfileQuery>(query)
                .ConfigureAwait(false);

            //todo: auto map result to profile

            return new Profile
            {
                FirstName = result.FirstName,
                Lastname = result.LastName,
                DateOfBirth = result.BirthDate
            };
        }

        public Task<IEnumerable<Measurement>> GetMeasurements(string userId, MeasurementTypes type)
        {
            throw new System.NotImplementedException();
        }
    }
}