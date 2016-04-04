using System;
using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidatorFactory : IValidatorFactory
    {
        private readonly FluentValidation.IValidatorFactory _validatorFactory;

        public FluentValidationValidatorFactory(FluentValidation.IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IValidator<T> CreateValidator<T>()
        {
            var fluentValidator = _validatorFactory.GetValidator<T>();
        }

        public IValidator CreateValidator(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
