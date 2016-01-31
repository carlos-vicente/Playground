using System;
using System.Threading.Tasks;
using Playground.Core.Logging;

namespace Playground.Logging.Serilog
{
    public class SerilogLogger : ILogger
    {
        private readonly global::Serilog.ILogger _logger;

        public SerilogLogger(global::Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public Task Information(string message, params object[] propertyValues)
        {
            _logger.Information(message, propertyValues);
            return Task.FromResult(1);
        }

        public Task Debug(string message, params object[] propertyValues)
        {
            _logger.Debug(message, propertyValues);
            return Task.FromResult(1);
        }

        public Task Warning(string message, params object[] propertyValues)
        {
            _logger.Warning(message, propertyValues);
            return Task.FromResult(1);
        }

        public Task Warning(Exception ex, string message, params object[] propertyValues)
        {
            _logger.Warning(ex, message, propertyValues);
            return Task.FromResult(1);
        }

        public Task Error(string message, params object[] propertyValues)
        {
            _logger.Error(message, propertyValues);
            return Task.FromResult(1);
        }

        public Task Error(Exception ex, string message, params object[] propertyValues)
        {
            _logger.Error(ex, message, propertyValues);
            return Task.FromResult(1);
        }
    }
}
