using System;

namespace Playground.Domain
{
    public abstract class Entity : IEquatable<Entity>
    {
        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }

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
