using System;
using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidatorFactory : IValidatorFactory
    {
        private readonly FluentValidation.IValidatorFactory _validatorFactory;

        public FluentValidationValidatorFactory(
            FluentValidation.IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IValidator<T> CreateValidator<T>()
        {
            var fluentValidator = _validatorFactory
                .GetValidator<T>();

            if(fluentValidator == null)
                throw new InvalidOperationException(
                    $"Could not find validator for type {typeof(T).Name}");

            return new FluentValidationValidator<T>(fluentValidator);
        }

        public IValidator CreateValidator(Type type)
        {
            var fluentValidator = _validatorFactory
                .GetValidator(type);

            if (fluentValidator == null)
                throw new InvalidOperationException(
                    $"Could not find validator for type {type.Name}");

            return new FluentValidationValidator(fluentValidator);
        }
    }
}
