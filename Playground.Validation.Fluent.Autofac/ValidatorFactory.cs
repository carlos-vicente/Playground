using System;
using FluentValidation;
using Playground.DependencyResolver.Contracts;

namespace Playground.Validation.Fluent.Autofac
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IDependencyResolver _dependencyResolver;

        public ValidatorFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IValidator<T> GetValidator<T>()
        {
            return _dependencyResolver.Resolve<IValidator<T>>();
        }

        public IValidator GetValidator(Type type)
        {
            return _dependencyResolver.Resolve(type) as IValidator;
        }
    }
}
