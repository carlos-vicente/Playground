using System;
using System.Threading.Tasks;

namespace Playground.Core.Logging
{
    public interface ILogger
    {
        Task Information(string message, params object[] propertyValues);
        Task Debug(string message, params object[] propertyValues);
        Task Warning(string message, params object[] propertyValues);
        Task Warning(Exception ex, string message, params object[] propertyValues);
        Task Error(string message, params object[] propertyValues);
        Task Error(Exception ex, string message, params object[] propertyValues);
    }
}
