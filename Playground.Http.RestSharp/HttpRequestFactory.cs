using System;
using System.Reflection;
using RestSharp;

namespace Playground.Http.RestSharp
{
    public class HttpRequestFactory : IHttpRequestFactory
    {
        public IRestRequest CreateGetRequest<TRequest>(
            string baseUrl,
            string urlFormat,
            TRequest request)
        {
            var resourceUrl = GetFormattedUrl(urlFormat, request);
            var resource = new Uri(new Uri(baseUrl), resourceUrl);

            return new RestRequest(resource, Method.GET);
        }

        public IRestRequest CreatePostRequest<TRequest>(
            string baseUrl,
            string urlFormat,
            TRequest request)
        {
            var resourceUrl = GetFormattedUrl(urlFormat, request);
            var resource = new Uri(new Uri(baseUrl), resourceUrl);

            var restRequest = new RestRequest(resource, Method.GET);

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