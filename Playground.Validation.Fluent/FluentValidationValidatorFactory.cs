using System;
using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidatorFactory : IValidatorFactory
    {
        public IValidator<T> CreateValidator<T>()
        {
            throw new NotImplementedException();
        }

        public IValidator CreateValidator(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
