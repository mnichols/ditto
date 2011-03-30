using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
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

        public Result TryResolve(IResolutionContext context)
        {
            if (sourced != null)
                return sourced.TryResolve(context);
            return UsingReflection(context.SourceType, context.Source);
        }


        private Result UsingReflection(Type sourceType, object target)
        {
            /*this is a bottleneck if caching hasnt been done (SourcedPropertyNameResolver)*/
            var prop = sourceType.GetProperty(sourcePropertyName);
            if (prop == null)
                throw new MappingExecutionException("Cannot find property '{0}' on '{1}'", sourcePropertyName,
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