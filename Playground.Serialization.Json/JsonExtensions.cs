using Jil;

namespace Playground.Serialization.Json
{
    public static class JsonExtensions
    {
        private static readonly Options Options = Options.IncludeInheritedUtcCamelCase;

        public static string ToJson(this object toFormat)
        {
            return JSON.SerializeDynamic(toFormat, Options);
        }
    }
}
