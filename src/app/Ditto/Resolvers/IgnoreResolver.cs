using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    /// Does nothing. It is important to note that the resolution is considered to be <c>Result.Unresolved</c>.
    /// </summary>
    public class IgnoreResolver:IResolveValue
    {
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return Result.Unresolved;
        }
    }
}