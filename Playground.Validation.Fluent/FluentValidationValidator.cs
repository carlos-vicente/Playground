using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidator : IValidator
    {
        public void Validate(object objectToValidate)
        {
            throw new System.NotImplementedException();
        }
    }

    public class FluentValidationValidator<TEntity> : IValidator<TEntity>
    {
        public void Validate(TEntity objectToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}