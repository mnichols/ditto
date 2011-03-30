using Ditto.Internal;

namespace Ditto.Resolvers
{
    public interface IDependOnDestinationPropertyState
    {
        Result ResolveBasedOn(IResolutionContext context,IDescribeMappableProperty destinationProperty);
    }
}