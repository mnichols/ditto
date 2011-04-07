using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class IgnoreResolver:IResolveValue
    {
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return Result.Unresolved;
        }
    }
}