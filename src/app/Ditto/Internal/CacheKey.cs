using System;

namespace Ditto.Internal
{
    internal class CacheKey
    {
        private Type type;
        private string name;

        public CacheKey(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            this.type = type;
            this.name = name;
        }

        public bool Equals(CacheKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.type, type) && Equals(other.name, name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CacheKey)) return false;
            return Equals((CacheKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (type.GetHashCode() * 397) ^ name.GetHashCode();
            }
        }
    }
}