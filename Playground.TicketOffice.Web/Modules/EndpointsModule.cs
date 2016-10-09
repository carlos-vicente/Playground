using System.CodeDom;
using Autofac;
using Playground.Http;
using Playground.Http.RestSharp;
using Playground.TicketOffice.Web.Configuration;
using RestSharp;

namespace Playground.TicketOffice.Web.Modules
{
    public class EndpointsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HttpRequestFactory>()
                .As<IHttpRequestFactory>()
                .InstancePerLifetimeScope();

            //builder
            //    .RegisterType<RestClient>()
            //    .As<IRestClient>();

            builder
                .RegisterType<HttpClient>()
                .Named<IHttpClient>(RegistrationConstants.MovieTheaterClientName)
                .WithParameter(
                    (info, context) => info.ParameterType.FullName == typeof(IRestClient).FullName,
                    (info, context) =>
                    {
                        var url = context
                            .Resolve<IEndpointsConfiguration>()
                            .MovieTheaterEndpoint;

                        return new RestClient(url) as IRestClient;
                    })
                .InstancePerLifetimeScope();
        }
    }
}