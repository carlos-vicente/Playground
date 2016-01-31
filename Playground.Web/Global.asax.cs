using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Playground.DependencyResolver.Autofac;
using Playground.QueryService.InMemory.Autofac;
using Playground.Web.Services;
using Playground.Web.Services.Contracts;
using AutofacDependencyResolver = Autofac.Integration.Mvc.AutofacDependencyResolver;

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

            var executingAssembly = Assembly.GetExecutingAssembly();

            // register MVC controllers
            builder.RegisterControllers(executingAssembly);

            // register WebApi controllersx
            builder.RegisterApiControllers(executingAssembly);

            builder.RegisterModule<AutofacDependencyResolverModule>();
            builder.RegisterModule<QueryServiceModule>();

            builder
                .RegisterType<UserService>()
                .As<IUserService>()
                .InstancePerLifetimeScope();

            // register the generic container to both MVC and WebApi pipelines
            var container = builder.Build();

            System.Web.Mvc.DependencyResolver
                .SetResolver(new AutofacDependencyResolver(container));

            GlobalConfiguration
                .Configuration
                .DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}