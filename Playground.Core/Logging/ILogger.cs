using System;

namespace Playground.Core.Logging
{
    public interface ILogger
    {
        void Information(string message, params object[] propertyValues);
        void Debug(string message, params object[] propertyValues);
        void Warning(string message, params object[] propertyValues);
        void Warning(Exception ex, string message, params object[] propertyValues);
        void Error(string message, params object[] propertyValues);
        void Error(Exception ex, string message, params object[] propertyValues);
    }
}
