using System.Collections;

namespace Ditto.Internal
{
    public class ListPropertyCriterion:IPropertyCriterion
    {
        public bool IsSatisfiedBy(IDescribeMappableProperty property)
        {
            return typeof (IList).IsAssignableFrom(property.PropertyType);
        }
    }
}