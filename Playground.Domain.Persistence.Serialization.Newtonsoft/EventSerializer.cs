using System;
using Newtonsoft.Json;
using Playground.Domain.Persistence.Events;

namespace Playground.Domain.Persistence.Serialization.Newtonsoft
{
    public class EventSerializer : IEventSerializer
    {
        private static readonly JsonSerializerSettings Settings;

        static EventSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
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
            return JsonConvert.DeserializeObject(rep, objectType, Settings);
        }

        public TObject Deserialize<TObject>(string rep)
        {
            return JsonConvert.DeserializeObject<TObject>(rep, Settings);
        }
    }
}
