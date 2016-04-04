using Jil;
using Playground.Domain.Persistence.Events;

namespace Playground.Serialization.Jil
{
    public class EventSerializer : IEventSerializer
    {
        public string Serialize(object obj)
        {
            return JSON.SerializeDynamic(obj);
        }

        public string Serialize<TObject>(TObject obj)
        {
            return JSON.Serialize(obj);
        }

        public object Deserialize(string rep)
        {
            return JSON.DeserializeDynamic(rep);
        }

        public TObject Deserialize<TObject>(string rep)
        {
            return JSON.Deserialize<TObject>(rep);
        }
    }
}
