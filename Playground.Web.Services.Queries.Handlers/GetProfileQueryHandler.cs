using System;
using System.Threading.Tasks;
using Playground.QueryService.Contracts;

namespace Playground.Web.Services.Queries.Handlers
{
    public class GetProfileQueryHandler : IAsyncQueryHandler<GetProfileQuery, GetProfileQueryResult>
    {
        public Task<GetProfileQueryResult> Handle(GetProfileQuery query)
        {
            // TODO: implement query stack data access in Playground.Data.Read and then use it here

            return Task.FromResult(new GetProfileQueryResult
            {
                BirthDate = DateTime.Today.AddYears(-30),
                FirstName = "Tha",
                LastName = "Dude"
            });
        }
    }
}