using System.IO;
using System.Linq;
using System.Web;
using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Playground.TicketOffice.Web.ReactConfig), "Configure")]

namespace Playground.TicketOffice.Web
{
	public static class ReactConfig
	{
		public static void Configure()
		{
            // If you want to use server-side rendering of React components, 
            // add all the necessary JavaScript files here. This includes 
            // your components as well as all of their dependencies.
            // See http://reactjs.net/ for more information. Example:

		    string path = $"{HttpRuntime.AppDomainAppPath}Scripts\\Components";

            var entries = Directory
		        .GetFileSystemEntries(path, "*.jsx", SearchOption.AllDirectories);

		    var configuration = ReactSiteConfiguration.Configuration;

		    var virtualPaths = entries
		        .Select(e => e.Replace(HttpRuntime.AppDomainAppPath, "~\\"));

		    foreach (var component in virtualPaths)
		    {
                configuration.AddScript(component);
		    }

            // If you use an external build too (for example, Babel, Webpack,
            // Browserify or Gulp), you can improve performance by disabling 
            // ReactJS.NET's version of Babel and loading the pre-transpiled 
            // scripts. Example:
            //ReactSiteConfiguration.Configuration
            //	.SetLoadBabel(false)
            //	.AddScriptWithoutTransform("~/Scripts/bundle.server.js")
        }
	}
}