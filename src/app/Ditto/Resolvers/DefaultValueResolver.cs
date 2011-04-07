using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class DefaultValueResolver:IResolveValue
    {
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return new Result(true, destinationProperty.PropertyType.DefaultValue());
        }

    }
}