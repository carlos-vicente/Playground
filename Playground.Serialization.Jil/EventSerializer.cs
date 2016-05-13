using Jil;
using Playground.Domain.Persistence.Events;

namespace Playground.Serialization.Jil
{
    public class EventSerializer : IEventSerializer
    {
        private static readonly Options Options;

        static EventSerializer()
        {
           Options = Options.IncludeInheritedUtcCamelCase;
        }

        public string Serialize(object obj)
        {
            return JSON.SerializeDynamic(obj);
        }

        public string Serialize<TObject>(TObject obj)
        {
            return JSON.Serialize<TObject>(obj, Options);
        }

        public object Deserialize(string rep)
        {
            return JSON.DeserializeDynamic(rep, Options);
        }

        public TObject Deserialize<TObject>(string rep)
        {
            return JSON.Deserialize<TObject>(rep, Options);
        }
    }
}
