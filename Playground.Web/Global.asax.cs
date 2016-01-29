using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;

namespace Playground.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var builder = new ContainerBuilder();

            // register MVC controllers
            builder.RegisterControllers(typeof(HttpApplication).Assembly);

            // register WebApi controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // register the generic container to both MVC and WebApi pipelines
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration
                .Configuration
                .DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}