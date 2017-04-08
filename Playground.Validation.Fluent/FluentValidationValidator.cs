using System;
using System.Collections.Generic;
using System.Linq;
using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidator : IValidator
    {
        private readonly FluentValidation.IValidator _validator;

        public FluentValidationValidator(FluentValidation.IValidator validator)
        {
            _validator = validator;
        }

        public void Validate(object objectToValidate)
        {
            var result = _validator
                .Validate(objectToValidate);

            if (!result.IsValid)
            {
                throw new ValidationException(
                    $"Error validating instance of {objectToValidate.GetType().Name}",
                    result.Errors.Select(Mapping.FailureTransformerFunc));
            }
        }

        public void ValidateAll(ICollection<object> objectsToValidate)
        {
            var canValidateAll = objectsToValidate
                .All(obj => _validator
                    .CanValidateInstancesOfType(obj.GetType()));

            if (!canValidateAll)
                throw new ArgumentException(
                    $"Not all objects can be validated by {_validator.GetType().Name}");

            var errors = new List<string>();

            foreach (var obj in objectsToValidate)
            {
                var result = _validator.Validate(obj);

                if (!result.IsValid)
                {
                    errors.AddRange(
                        result
                            .Errors
                            .Select(Mapping.FailureTransformerFunc));
                }
            }

            if (errors.Any())
            {
                throw new ValidationException(
                    "Error validating collection of objects", 
                    errors);
            }
        }
    }

    public class FluentValidationValidator<TEntity> : IValidator<TEntity>
    {
        private readonly FluentValidation.IValidator<TEntity> _validator;

        public FluentValidationValidator(FluentValidation.IValidator<TEntity> validator)
        {
            _validator = validator;
        }

        public void Validate(TEntity objectToValidate)
        {
            var result = _validator
                .Validate(objectToValidate);

            if (!result.IsValid)
            {
                throw new ValidationException(
                    $"Error validating instance of {typeof(TEntity).Name}",
                    result.Errors.Select(Mapping.FailureTransformerFunc));
            }
        }

        public void ValidateAll(ICollection<TEntity> objectsToValidate)
        {
            var errors = new List<string>();

            foreach (var obj in objectsToValidate)
            {
                var result = _validator.Validate(obj);

                if (!result.IsValid)
                {
                    errors.AddRange(
                        result
                            .Errors
                            .Select(Mapping.FailureTransformerFunc));
                }
            }

            if (errors.Any())
            {
                throw new ValidationException(
                    "Error validating collection of objects",
                    errors);
            }
        }
    }
}