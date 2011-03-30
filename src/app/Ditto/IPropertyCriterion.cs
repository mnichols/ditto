using Ditto.Internal;

namespace Ditto
{
    public interface IPropertyCriterion
    {
        bool IsSatisfiedBy(IDescribeMappableProperty property);
    }
}