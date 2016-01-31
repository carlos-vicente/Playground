using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Serilog;
using Serilog.Enrichers;

namespace Playground.Logging.Serilog.Autofac
{
    public class SerilogLoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.With<ThreadIdEnricher>();

            Log.Logger = configuration.CreateLogger();

            // configure Log to return our Serilog logger abstraction
            Core.Logging.Log.Customize(type => new SerilogLogger(Log.ForContext(type)));
        }

        protected override void AttachToComponentRegistration(
           IComponentRegistry componentRegistry,
           IComponentRegistration registration)
        {
            var type = registration.Activator.LimitType;

            if (HasPropertyDependencyOnLogger(type))
                registration.Activated += InjectLoggerViaProperty;

            if (HasConstructorDependencyOnLogger(type))
                registration.Preparing += InjectLoggerViaConstructor;
        }

        private static bool HasPropertyDependencyOnLogger(Type type)
        {
            return type.GetProperties().Any(property => property.CanWrite && property.PropertyType == typeof(Core.Logging.ILogger));
        }

        private static bool HasConstructorDependencyOnLogger(Type type)
        {
            return type.GetConstructors()
                .SelectMany(constructor => constructor
                    .GetParameters()
                    .Where(parameter => parameter.ParameterType == typeof(Core.Logging.ILogger)))
                .Any();
        }

        private void InjectLoggerViaProperty(object sender, ActivatedEventArgs<object> args)
        {
            var type = args.Instance.GetType();

            var propertyInfo = type.GetProperties()
                .First(x => x.CanWrite && x.PropertyType == typeof(Core.Logging.ILogger));

            propertyInfo.SetValue(args.Instance, Core.Logging.Log.ForContext(type), null);
        }

        private void InjectLoggerViaConstructor(object sender, PreparingEventArgs args)
        {
            var type = args.Component.Activator.LimitType;

            args.Parameters = args.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (parameter, context) => parameter.ParameterType == typeof(Core.Logging.ILogger),
                        (p, i) => Core.Logging.Log.ForContext(type))
                });
        }
    }
}
