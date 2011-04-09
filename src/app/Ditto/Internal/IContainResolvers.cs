namespace Ditto.Internal
{
    public interface IContainResolvers
    {
        bool WillResolve(IDescribeMappableProperty mappableProperty);
        IResolveValue GetResolver(IDescribeMappableProperty mappableProperty);
    }
}