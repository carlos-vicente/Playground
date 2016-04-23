using System.Collections.Generic;

namespace Playground.Core.Validation
{
    public interface IValidator
    {
        void Validate(object objectToValidate);
        void ValidateAll(ICollection<object> objectsToValidate);
    }

    public interface IValidator<TEntity>
    {
        void Validate(TEntity objectToValidate);
        void ValidateAll(ICollection<TEntity> objectToValidate);
    }
}