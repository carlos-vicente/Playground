using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Playground.Core.Validation
{
    public class ValidationException : Exception
    {
        private IEnumerable<string> _errors;

        public ValidationException()
        {
            _errors = new List<string>();
        }

        public ValidationException(string message)
            : base(message)
        {
            _errors = new List<string>();
        }

        public ValidationException(string message, IEnumerable<string> errors)
            : base(message)
        {
            _errors = errors;
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
            _errors = new List<string>();
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}