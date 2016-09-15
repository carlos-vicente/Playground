using RestSharp;

namespace Playground.Http.RestSharp
{
    public interface IHttpRequestFactory
    {
        IRestRequest CreateGetRequest<TRequest>(string urlFormat, TRequest request);

        IRestRequest CreatePostRequest<TRequest>(string urlFormat, TRequest request);
    }
}