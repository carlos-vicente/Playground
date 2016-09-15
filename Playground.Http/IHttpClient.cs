using System.Net;
using System.Threading.Tasks;
using Playground.Api.Contracts;

namespace Playground.Http
{
    public interface IHttpClient
    {
        Task<TResponse> Get<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest;

        Task Post<TRequest>(TRequest request, params HttpStatusCode[] acceptedStatusCodes)
            where TRequest : IRequest;
    }
}