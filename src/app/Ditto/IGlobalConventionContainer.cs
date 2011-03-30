namespace Ditto
{
    public interface IGlobalConventionContainer
    {
        void AddConverter(IConvertValue converter);
        void AddResolver(IPropertyCriterion propertyCriterion, IResolveValue resolver);
    }
}