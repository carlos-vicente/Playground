namespace Playground.Domain.Model
{
    public abstract class Identity : IIdentity
    {
        protected Identity(string id)
        {
            Id = id;
        }

        public virtual string Id { get; }
    }
}