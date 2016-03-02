namespace Playground.Domain.Persistence
{
    public interface IEventSerializer
    {
        string Serialize(object obj);

        string Serialize<TObject>(TObject obj);

        object Deserialize(string rep);

        TObject Deserialize<TObject>(string rep);
    }
}