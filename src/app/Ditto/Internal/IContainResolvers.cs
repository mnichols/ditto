namespace Ditto.Internal
{
    public interface IContainResolvers
    {
        bool WillResolve(IDescribeMappableProperty destinationProperty);
        IResolveValue GetResolver(IDescribeMappableProperty destinationProperty);
    }
}