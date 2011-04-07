using System;
using Ditto.Internal;

namespace Ditto.Criteria
{
    public class TypePropertyCriterion:IPropertyCriterion
    {
        private readonly Type targetType;

        public TypePropertyCriterion(Type targetType)
        {
            this.targetType = targetType;
        }

        public bool IsSatisfiedBy(IDescribeMappableProperty property)
        {
            return property.PropertyType.IsOneOf(targetType);
        }
    }
}