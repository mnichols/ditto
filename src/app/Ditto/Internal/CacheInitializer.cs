namespace Ditto.Internal
{
    public class CacheInitializer : IVisitCacheable
    {
        private readonly ICacheInvocation cache;

        public CacheInitializer(ICacheInvocation cache)
        {
            this.cache = cache;
        }

        public void Visit(SourcedPropertyNameResolver propertyNameResolver)
        {
            propertyNameResolver.GetValue = cache.CacheGet(propertyNameResolver.SourceType, propertyNameResolver.PropertyName);
        }

        public void Visit(IDescribeMappableProperty mappableProperty)
        {
            cache.CacheSet(mappableProperty.DeclaringType, mappableProperty.Name);
        }
    }
}