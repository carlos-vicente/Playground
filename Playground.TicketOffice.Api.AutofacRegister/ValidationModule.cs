using Autofac;
using Playground.DependencyResolver.Autofac;
using Playground.Validation.Fluent;
using Playground.Validation.Fluent.Autofac;

namespace Playground.TicketOffice.Api.AutofacRegister
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ValidatorFactory>()
                .As<FluentValidation.IValidatorFactory>();

            builder
                .RegisterGenerics(typeof (FluentValidation.AbstractValidator<>));

            builder
                .RegisterType<FluentValidationValidatorFactory>()
                .As<Core.Validation.IValidatorFactory>();
        }
    }
}