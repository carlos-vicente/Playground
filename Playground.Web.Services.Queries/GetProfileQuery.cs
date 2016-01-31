using Playground.QueryService.Contracts;

namespace Playground.Web.Services.Queries
{
    public class GetProfileQuery : IQuery<GetProfileQueryResult>
    {
        public string UserId { get; set; } 
    }
}