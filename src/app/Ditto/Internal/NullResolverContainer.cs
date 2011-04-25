using System;

namespace Ditto.Internal
{
    public class NullResolverContainer : IContainResolvers
    {
        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            return false;
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            return NullResolver.Instance;
        }

    }
}