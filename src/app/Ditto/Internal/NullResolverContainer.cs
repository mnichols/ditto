namespace Ditto.Internal
{
    public class NullResolverContainer : IContainResolvers
    {
        public bool WillResolve(IDescribeMappableProperty mappableProperty)
        {
            return false;
        }

        public IResolveValue GetResolver(IDescribeMappableProperty mappableProperty)
        {
            return NullResolver.Instance;
        }
    }
}