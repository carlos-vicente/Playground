using System;

namespace Playground.Domain.Model
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public bool Equals(Entity other)
        {
            if (other == null)
                return false;

            return ReferenceEquals(this, other)
                   || Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
