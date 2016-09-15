using System.Reflection;
using RestSharp;

namespace Playground.Http.RestSharp
{
    public class HttpRequestFactory : IHttpRequestFactory
    {
        public IRestRequest CreateGetRequest<TRequest>(string urlFormat, TRequest request)
        {
            var finalUrl = GetFormattedUrl(urlFormat, request);

            return new RestRequest(finalUrl, Method.GET);
        }

        public IRestRequest CreatePostRequest<TRequest>(string urlFormat, TRequest request)
        {
            var finalUrl = GetFormattedUrl(urlFormat, request);

            var restRequest = new RestRequest(finalUrl, Method.GET);

            restRequest.AddJsonBody(request);

            return restRequest;
        }

        private string GetFormattedUrl(string urlFormat, object request)
        {
            var requestType = request.GetType();

            var finalUrl = urlFormat;

            int startingBracketsIndex,
                closingBracketsIndex,
                lastIndex = -1;
            do
            {
                startingBracketsIndex = urlFormat.IndexOf('{', lastIndex + 1);

                if (startingBracketsIndex > lastIndex)
                {
                    closingBracketsIndex = urlFormat.IndexOf('}', startingBracketsIndex);

                    var fieldName = urlFormat
                        .Substring(
                            startingBracketsIndex + 1,
                            closingBracketsIndex - startingBracketsIndex - 1);

                    var property = requestType
                        .GetProperty(fieldName, BindingFlags.IgnoreCase);

                    finalUrl = finalUrl.Replace(
                        string.Format("{{{0}}}", fieldName),
                        property.GetValue(request).ToString());

                    lastIndex = closingBracketsIndex;
                }

            } while (startingBracketsIndex != -1);

            return finalUrl;
        }
    }
}