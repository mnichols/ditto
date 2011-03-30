namespace Ditto.Internal
{
    public interface IApplyConventions
    {
        void Apply(IPropertyCriterion propertyCriterion, IResolveValue resolver);
    }
}