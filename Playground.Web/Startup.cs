using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Playground.Web.Startup))]
namespace Playground.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}