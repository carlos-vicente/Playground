using System.Configuration;

namespace Playground.TicketOffice.Web.Configuration
{
    public class AppSettingsEndpointsConfiguration : IEndpointsConfiguration
    {
        public string MovieTheaterEndpoint { get; private set; }

        public AppSettingsEndpointsConfiguration()
        {
            MovieTheaterEndpoint = ConfigurationManager
                .AppSettings["endpoints:movieTheater"];
        }
    }
}