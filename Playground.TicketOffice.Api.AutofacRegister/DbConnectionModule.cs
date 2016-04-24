﻿using System;
using System.Configuration;
using System.Data;
using Autofac;
using Npgsql;
using Playground.Data.Contracts;
using Playground.Data.Dapper;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class DbConnectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationManager.AppSettings["host"],
                Database = ConfigurationManager.AppSettings["database"],
                Username = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],

                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            builder
                .RegisterType<NpgsqlConnection>()
                .As<IDbConnection>()
                .InstancePerLifetimeScope();
            
            //builder
            //    .Register<Func<string, IDbConnection>>(ctx =>
            //    {
            //        return connectionString => ctx
            //            .Resolve<IDbConnection>(
            //                new NamedParameter("connectionString", connectionString));
            //    });

            builder
                .Register<Func<string, IDbConnection>>(ctx =>
                {
                    var context = ctx.Resolve<IComponentContext>();

                    return connectionString => context
                        .Resolve<IDbConnection>(
                            new NamedParameter("connectionString", connectionString));
                });

            builder
                .RegisterType<Connection>()
                .As<IConnection>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConnectionFactory>()
                .As<IConnectionFactory>()
                .InstancePerLifetimeScope()
                .WithParameter("connectionString", connectionStringBuilder.ConnectionString);
        }
    }
}