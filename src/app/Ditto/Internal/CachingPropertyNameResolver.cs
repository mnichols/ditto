using System;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class CachingPropertyNameResolver
    {
        
        public void TryCache(Type sourceType,IVisitCacheable visitor,IResolveValue resolver)
        {
            var propertyNameResolver = resolver as PropertyNameResolver;
            if(propertyNameResolver==null)
                return;
            var sourced = propertyNameResolver.SourcedBy(sourceType);
            visitor.Visit(sourced);
        }
    }
}