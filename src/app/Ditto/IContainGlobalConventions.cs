namespace Ditto
{
    public interface IContainGlobalConventions
    {
        void AddConverter(IConvertValue converter);
        void AddResolver(IPropertyCriterion propertyCriterion, IResolveValue resolver);
    }
}