using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    /// Resolved based on the provided source property name. 
    /// </summary>
    public class PropertyNameResolver : IResolveValue
    {
        private readonly string sourcePropertyName;
        private SourcedPropertyNameResolver sourced;

        public PropertyNameResolver(string sourcePropertyName)
        {
            if (string.IsNullOrEmpty(sourcePropertyName))
                throw new ArgumentNullException("sourcePropertyName");
            this.sourcePropertyName = sourcePropertyName;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if (sourced != null)
                return sourced.TryResolve(context, destinationProperty);
            return UsingReflection(context.SourceType, context.Source);
        }


        private Result UsingReflection(Type sourceType, object target)
        {
            /*this is a bottleneck if caching hasnt been done (SourcedPropertyNameResolver)*/
            var prop = sourceType.GetProperty(sourcePropertyName);
            if (prop == null)
                throw new DittoExecutionException("Cannot find property '{0}' on '{1}'", sourcePropertyName,
                                                    sourceType);
            return new Result(true, prop.GetValue(target, null));
        }

        internal SourcedPropertyNameResolver SourcedBy(Type sourceType)
        {
            sourced = new SourcedPropertyNameResolver(sourceType, sourcePropertyName);
            return sourced;
        }
    }
}