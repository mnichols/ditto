using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    ///   This resolver won't overwrite a destination value if one exists. It first checks for the default value by the field's Type to determine
    ///   if it should resolve. If it <i>is</i> resolvable (destination has a default value), then it uses the wrapped (<b>inner</b>) resolver for resolution; otherwise, 
    ///   it gets <c>Result.Unresolved</c>.
    ///   <seealso cref = "Result.Unresolved">Result.Unresolved</seealso>
    ///   <seealso cref = "InternalTypeExtensions.DefaultValue">DefaultValue extension</seealso>
    /// </summary>
    public class ImmutableDestinationResolver : IResolveValue
    {
        private IResolveValue inner;

        public ImmutableDestinationResolver(IResolveValue inner)
        {
            this.inner = inner;
        }
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            var destValue = context.GetDestinationPropertyValue(destinationProperty.Name);
            if (destinationProperty.PropertyType.IsDefaultOrEmpty(destValue) == false)
                return Result.Unresolved;
            return inner.TryResolve(context, destinationProperty);
        }
    }
}