using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Playground.Core.Serialization;

namespace Playground.Serialization.Newtonsoft
{
    public class ObjectSerializer : IObjectSerializer
    {
        private static readonly JsonSerializerSettings Settings;

        static ObjectSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                //TypeNameHandling = TypeNameHandling.All, // to always know which type was serialized
                //Converters = new List<JsonConverter>
                //{
                //    new StringEnumConverter(),
                //    new GuidConverter()
                //}
            };
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public string Serialize<TObject>(TObject obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public object Deserialize(string rep, Type objectType)
        {
            var obj1 = JsonConvert.DeserializeObject(rep, objectType);

            var obj2 = JsonConvert.DeserializeObject(rep, objectType, Settings);

            return obj2;
        }

        public TObject Deserialize<TObject>(string rep)
        {
            return JsonConvert.DeserializeObject<TObject>(rep, Settings);
        }
    }
}
