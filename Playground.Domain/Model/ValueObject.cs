using System;
using System.Collections.Generic;
using System.Reflection;

namespace Playground.Domain.Model
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        private const int StartValue = 17;
        private const int Multiplier = 59;

        public static bool operator ==(ValueObject x, ValueObject y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ValueObject x, ValueObject y)
        {
            return !(x.Equals(y));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as ValueObject;

            return Equals(other);
        }

        public virtual bool Equals(ValueObject other)
        {
            if (other == null)
                return false;

            var thisType = GetType();
            var otherType = other.GetType();

            if (thisType != otherType)
                return false;

            var fields = thisType
                .GetFields(BindingFlags.Instance
                           | BindingFlags.NonPublic
                           | BindingFlags.Public);

            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var fields = GetFields();

            var hashCode = StartValue;

            foreach (var field in fields)
            {
                var value = field.GetValue(this);

                if (value != null)
                    hashCode = (hashCode * Multiplier) + value.GetHashCode();
            }

            return hashCode;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var t = this.GetType();

            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Instance
                                            | BindingFlags.NonPublic
                                            | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }
    }
}
