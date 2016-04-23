using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Playground.Core.Validation
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; private set; }

        public ValidationException()
        {
            Errors = new List<string>();
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<string>();
        }

        public ValidationException(string message, IEnumerable<string> errors)
            : base(message)
        {
            Errors = errors;
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
            Errors = new List<string>();
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}