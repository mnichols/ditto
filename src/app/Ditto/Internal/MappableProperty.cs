using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Ditto.Internal
{
    public class MappableProperty : IDescribeMappableProperty,ICacheable
    {
        public MappableProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            Name = propertyInfo.Name;
            PropertyType = propertyInfo.PropertyType;
            DeclaringType = propertyInfo.DeclaringType;
        }

        protected MappableProperty(Type declaringType, Type propertyType, string name)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            PropertyType = propertyType;
            Name = name;
            DeclaringType = declaringType;
        }


        public Type DeclaringType { get; private set; }
        public string Name { get; private set; }
        public Type PropertyType { get; private set; }

        public static MappableProperty For<TDest>(Expression<Func<TDest, object>> property)
        {
            var info = Reflect.GetProperty(property);
            return new MappableProperty(info);
        }


        public override string ToString()
        {
            return DeclaringType + ":" + Name;
        }

        public bool Equals(IDescribeMappableProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.DeclaringType, DeclaringType) && Equals(other.Name, Name) &&
                   Equals(other.PropertyType, PropertyType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MappableProperty)) return false;
            return Equals((IDescribeMappableProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = DeclaringType.GetHashCode();
                result = (result*397) ^ Name.GetHashCode();
                result = (result*397) ^ PropertyType.GetHashCode();
                return result;
            }
        }

        public IDescribePropertyElement ElementAt(int index)
        {
            if (typeof (ICollection).IsAssignableFrom(PropertyType) == false)
                throw new NotSupportedException(DeclaringType + " is not a supported collection type");
            return new MappablePropertyElement(this, PropertyType/*collection type*/, PropertyType.ElementType(), index);
        }

        public void Accept(IVisitCacheable visitor)
        {
            visitor.Visit(this);
        }

        private class MappablePropertyElement : MappableProperty, IDescribePropertyElement
        {
            internal MappablePropertyElement(IDescribeMappableProperty parentProperty, Type collectionType, Type propertyType, int index)
                : base(collectionType, propertyType, collectionType.Name + ".Item")
            {
                Index = index;
            }

            public int Index { get; private set; }
        }
    }

    
}