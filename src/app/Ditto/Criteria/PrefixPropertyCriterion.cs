using Ditto.Internal;

namespace Ditto.Criteria
{
    public class PrefixPropertyCriterion:IPropertyCriterion
    {
        private string prefix;

        public PrefixPropertyCriterion(string prefix)
        {
            this.prefix = prefix;
        }

        public bool IsSatisfiedBy(IDescribeMappableProperty property)
        {
            return property.Name.StartsWith(prefix);
        }
    }
}