using System;

namespace Playground.Core.Serialization
{
    public interface IObjectSerializer
    {
        string Serialize(object obj);

        string Serialize<TObject>(TObject obj);

        object Deserialize(string rep, Type objectType);

        TObject Deserialize<TObject>(string rep);
    }
}