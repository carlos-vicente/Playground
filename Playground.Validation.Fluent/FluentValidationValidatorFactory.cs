using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playground.Core.Validation;

namespace Playground.Validation.Fluent
{
    public class FluentValidationValidatorFactory : IValidatorFactory
    {
        public IValidator<T> CreateValidator<T>()
        {
            throw new NotImplementedException();
        }

        public IValidator GetValidator(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
