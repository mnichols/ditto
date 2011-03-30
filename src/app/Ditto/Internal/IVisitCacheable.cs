namespace Ditto.Internal
{
    public interface IVisitCacheable
    {
        void Visit(SourcedPropertyNameResolver propertyNameResolver);
        void Visit(IDescribeMappableProperty mappableProperty);
    }
}