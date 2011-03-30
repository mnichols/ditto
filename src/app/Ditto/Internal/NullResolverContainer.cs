namespace Ditto.Internal
{
    public class NullResolverContainer : IResolverContainer
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