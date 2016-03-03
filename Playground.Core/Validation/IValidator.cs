using System.Collections.Generic;

namespace Playground.Core.Validation
{
    public interface IValidator
    {
        void Validate(object objectToValidate);
        void ValidateAll(IEnumerable<object> objectsToValidate);
    }

    public interface IValidator<in TEntity>
    {
        void Validate(TEntity objectToValidate);
        void ValidateAll(IEnumerable<TEntity> objectToValidate);
    }
}