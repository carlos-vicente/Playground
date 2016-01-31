using System;

namespace Playground.Core.Logging
{
    public static class Log
    {
        static Func<Type, ILogger> _loggerFactory;

        public static void Customize(Func<Type, ILogger> factory)
        {
            _loggerFactory = factory;
        }

        public static ILogger ForContext<T>()
        {
            return _loggerFactory(typeof(T));
        }

        public static ILogger ForContext(Type type)
        {
            return _loggerFactory(type);
        }
    }
}
