using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class DeferredConfigurationResolver:IResolveValue
    {
        private Func<IResolveValue> getResolver;
        private IResolveValue inner;

        public DeferredConfigurationResolver(Func<IResolveValue> getResolver)
        {
            this.getResolver = getResolver;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if(inner==null)
            {
                inner = getResolver();
            }
            return inner.TryResolve(context, destinationProperty);
        }
    }
}