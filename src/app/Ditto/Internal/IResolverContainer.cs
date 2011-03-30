namespace Ditto.Internal
{
    public interface IResolverContainer
    {
        bool WillResolve(IDescribeMappableProperty mappableProperty);
        IResolveValue GetResolver(IDescribeMappableProperty mappableProperty);
    }
}