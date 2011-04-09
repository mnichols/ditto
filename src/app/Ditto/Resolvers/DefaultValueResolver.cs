using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    /// Set destination property to its default value, based on the <c>PropertyType</c>
    /// </summary>
    public class DefaultValueResolver:IResolveValue
    {
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return new Result(true, destinationProperty.PropertyType.DefaultValue());
        }

    }
}