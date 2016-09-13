using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Autofac;
using Playground.Data.Contracts;
using Playground.Data.Dapper;
using Playground.TicketOffice.Theater.Data;
using Playground.TicketOffice.Theater.Data.Contracts;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class ReadModelConnectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["MovieTicketOfficeDb"]
                .ConnectionString;

            builder
                .Register<Func<string, IDbConnection>>(
                    ctx => connString => new SqlConnection(connString));

            builder
                .RegisterType<ConnectionFactory>()
                .As<IConnectionFactory>()
                .WithParameter(new TypedParameter(typeof(string), connectionString))
                .SingleInstance();

            builder
                .RegisterType<MovieTheaterRepository>()
                .As<IMovieTheaterRepository>()
                .InstancePerLifetimeScope();
        }
    }
}