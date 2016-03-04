namespace Playground.Domain.Persistence.Events
{
    public interface IEventSerializer
    {
        string Serialize(object obj);

        string Serialize<TObject>(TObject obj);

        object Deserialize(string rep);

        TObject Deserialize<TObject>(string rep);
    }
}