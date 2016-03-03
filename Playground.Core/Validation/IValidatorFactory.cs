using System;

namespace Playground.Core.Validation
{
    public interface IValidatorFactory
    {
        IValidator<T> CreateValidator<T>();
        IValidator CreateValidator(Type type);
    }
}