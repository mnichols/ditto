using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    ///   This resolver won't overwrite a destination value if one exists. It first checks for <c>null</c> and <c>string.IsNullOrEmpty</c> to determine
    ///   if it should resolve. If it <i>is</i> resolvable, then it uses the wrapped (<b>inner</b>) resolver for resolution; otherwise, 
    ///   it gets <c>Result.Unresolved</c>.
    ///   <seealso cref = "Result.Unresolved">Result.Unresolved</seealso>.
    /// </summary>
    public class ImmutableDestinationResolver : IResolveValue, IDependOnDestinationPropertyState
    {
        private IResolveValue inner;

        public ImmutableDestinationResolver(IResolveValue inner)
        {
            this.inner = inner;
        }

        public Result ResolveBasedOn(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            var destValue = context.GetDestinationPropertyValue(destinationProperty.Name);
            if (destinationProperty.PropertyType.IsDefaultOrEmpty(destValue)==false)
                return Result.Unresolved;
            return inner.TryResolve(context);
        }

        public Result TryResolve(IResolutionContext context)
        {
            throw new NotSupportedException("Use ResolveBasedOn instead");
        }
    }
}