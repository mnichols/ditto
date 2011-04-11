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
            propertyNameResolver.GetValue = cache.CacheGet(propertyNameResolver.SourceType,
                                                           propertyNameResolver.PropertyName);
        }

        public void Visit(IDescribeMappableProperty mappableProperty)
        {
            cache.CacheSet(mappableProperty.DeclaringType, mappableProperty.Name);
        }

        public void Visit(BindableConfiguration bindableConfiguration)
        {
            var mapped = ResolverPrecedence.ToResolverContainer(bindableConfiguration);
            foreach (var item in mapped)
            {
                item.Value.Source(bindableConfiguration.DestinationProperties);
                bindableConfiguration.SourceResolverContainer(item.Key, item.Value);
            }
        }
    }
}