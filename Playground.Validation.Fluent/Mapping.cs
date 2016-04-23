using System;
using FluentValidation.Results;

namespace Playground.Validation.Fluent
{
    internal static class Mapping
    {
        internal static readonly Func<ValidationFailure, string> FailureTransformerFunc =
            vf => vf.ToString();
    }
}