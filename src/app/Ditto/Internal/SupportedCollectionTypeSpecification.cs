using System;
using System.Collections;

namespace Ditto.Internal
{
    public class SupportedCollectionTypeSpecification
    {
        public bool IsElement(IDescribeMappableProperty destinationProperty)
        {
            return destinationProperty is IDescribePropertyElement;
        }
        public bool IsSatisfiedBy(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }
        public bool IsSatisfiedBy(object instance)
        {
            if (instance == null)
                return false;
            return IsSatisfiedBy(instance.GetType());
        }
        public int GetLength(object instance)
        {
            if (IsSatisfiedBy(instance) == false)
                return 0;
            var list = instance as IList;
            if (list != null)
                return list.Count;
            throw new NotSupportedException("Not sure how to find length of " + instance.GetType());
        }

        public object GetValue(object src, IDescribePropertyElement elementProperty)
        {
            if (elementProperty == null)
                return null;
            var temp = (IList) src;
            
            return typeof (IList).GetProperty("Item").GetValue(temp, new object[] {elementProperty.Index});
        }

    }
}