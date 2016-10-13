using System;
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

        public void Information(string message, params object[] propertyValues)
        {
            _logger.Information(message, propertyValues);
        }

        public void Debug(string message, params object[] propertyValues)
        {
            _logger.Debug(message, propertyValues);
        }

        public void Warning(string message, params object[] propertyValues)
        {
            _logger.Warning(message, propertyValues);
        }

        public void Warning(Exception ex, string message, params object[] propertyValues)
        {
            _logger.Warning(ex, message, propertyValues);
        }

        public void Error(string message, params object[] propertyValues)
        {
            _logger.Error(message, propertyValues);
        }

        public void Error(Exception ex, string message, params object[] propertyValues)
        {
            _logger.Error(ex, message, propertyValues);
        }
    }
}
