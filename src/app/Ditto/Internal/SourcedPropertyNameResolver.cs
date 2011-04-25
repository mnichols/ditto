using System;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    /// <summary>
    /// Same as <c>PropertyNameResolver</c> only allows reachability for caching.
    /// </summary>
    public class SourcedPropertyNameResolver:ICacheable,IResolveValue
    {
        private readonly PropertyNameResolver inner;
        internal Type SourceType { get; private set; }
        internal string PropertyName { get; private set; }

        internal GetValue GetValue { get; set; }

        public SourcedPropertyNameResolver(Type sourceType, PropertyNameResolver inner)
        {
            this.inner = inner;
            SourceType = sourceType;
            PropertyName = inner.SourcePropertyName;

        }

        public void Accept(IVisitCacheable visitor)
        {
                visitor.Visit(this);
        }

        public override string ToString()
        {
            return SourceType.FullName + "|" + PropertyName;
        }
        
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if(GetValue==null)
                throw new InvalidOperationException("GetValue was not assigned to this resolver.");
            if (context.Source == null)
                return Result.Unresolved;
            return new Result(true,GetValue(context.Source));
        }
    }
}