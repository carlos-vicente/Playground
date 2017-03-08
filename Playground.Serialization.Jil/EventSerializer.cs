using System;
using Jil;
using Playground.Core.Serialization;

namespace Playground.Serialization.Jil
{
    public class ObjectSerializer : IObjectSerializer
    {
        private static readonly Options Options;

        static ObjectSerializer()
        {
           Options = Options.UtcCamelCase;
        }

        public string Serialize(object obj)
        {
            return JSON.SerializeDynamic(obj, Options);
        }

        public string Serialize<TObject>(TObject obj)
        {
            return JSON.Serialize(obj, Options);
        }

        public object Deserialize(string rep, Type objectType)
        {
            return JSON.Deserialize(rep, objectType, Options);
        }

        public TObject Deserialize<TObject>(string rep)
        {
            return JSON.Deserialize<TObject>(rep, Options);
        }
    }
}
