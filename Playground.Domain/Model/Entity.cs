using System;

namespace Playground.Domain.Model
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; private set; }

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

    public abstract class EntityWithTypedIdentity<TIdentity> 
        : IEquatable<EntityWithTypedIdentity<TIdentity>>
        where TIdentity : IIdentity
    {
        public TIdentity Identity { get; private set; }

        protected EntityWithTypedIdentity(TIdentity identity)
        {
            Identity = identity;
        }

        public bool Equals(EntityWithTypedIdentity<TIdentity> other)
        {
            if (other == null)
                return false;

            return ReferenceEquals(this, other)
                   || Identity.Id.Equals(other.Identity.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        public override int GetHashCode()
        {
            return Identity.Id.GetHashCode();
        }
    }
}
