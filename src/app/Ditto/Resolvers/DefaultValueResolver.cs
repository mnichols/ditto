using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class DefaultValueResolver:IResolveValue,IDependOnDestinationPropertyState
    {
        public Result TryResolve(IResolutionContext context)
        {
            throw new NotImplementedException("Use ResolveBasedOn instead");
        }

        public Result ResolveBasedOn(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return new Result(true,destinationProperty.PropertyType.DefaultValue());
        }
    }
}