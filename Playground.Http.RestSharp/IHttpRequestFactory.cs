using RestSharp;

namespace Playground.Http.RestSharp
{
    public interface IHttpRequestFactory
    {
        IRestRequest CreateGetRequest<TRequest>(string baseUrl, string urlFormat, TRequest request);

        IRestRequest CreatePostRequest<TRequest>(string baseUrl, string urlFormat, TRequest request);
    }
}