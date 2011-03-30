using System;
using System.Linq.Expressions;
using Ditto.Internal;

namespace Ditto.Criteria
{
    public class PropertyNameCriterion : IPropertyCriterion
    {
        private string name;
        
        public PropertyNameCriterion(string name)
        {
            this.name = name;
        }

        public bool IsSatisfiedBy(IDescribeMappableProperty property)
        {
            return string.Equals(property.Name, name);
        }

        public static PropertyNameCriterion From<T>(Expression<Func<T, object>> property)
        {
            return new PropertyNameCriterion(Reflect.GetProperty(property).Name);
        }
    }
}