using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Playground.Api.Contracts;
using Playground.Serialization.Json;
using RestSharp;

namespace Playground.Http.RestSharp
{
    public class HttpClient : IHttpClient
    {
        private readonly IRestClient _restClient;
        private readonly IHttpRequestFactory _requestFactory;

        public HttpClient(
            IRestClient restClient,
            IHttpRequestFactory requestFactory)
        {
            _restClient = restClient;
            _requestFactory = requestFactory;
        }

        public async Task<TResponse> Get<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest
        {
            var url = request.GetRelativeUrl();

            var restRequest = _requestFactory
                .CreateGetRequest(url, request);

            var response = await _restClient
                .ExecuteGetTaskAsync<TResponse>(restRequest)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;

            var errorMessage = $"Error while performing a GET request on {url} with {Environment.NewLine}{request.ToJson()}{Environment.NewLine}Error: {response.ErrorMessage}";
            if (response.ErrorException == null)
                throw new ApplicationException(errorMessage);
            throw new ApplicationException(errorMessage, response.ErrorException);
        }

        public async Task Post<TRequest>(TRequest request, params HttpStatusCode[] acceptedStatusCodes) 
            where TRequest : IRequest
        {
            var url = request.GetRelativeUrl();

            var restRequest = _requestFactory
                .CreatePostRequest(url, request);

            var response = await _restClient
                .ExecutePostTaskAsync(restRequest)
                .ConfigureAwait(false);

            if (!acceptedStatusCodes.Contains(response.StatusCode))
            {
                var errorMessage = $"Error while performing a POST request on {url} with {Environment.NewLine}{request.ToJson()}{Environment.NewLine}Error: {response.ErrorMessage}";
                if (response.ErrorException == null)
                    throw new ApplicationException(errorMessage);
                throw new ApplicationException(errorMessage, response.ErrorException);
            }
        }
    }
}
