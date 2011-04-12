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
            if(IsCacheable())
                visitor.Visit(this);
        }

        public override string ToString()
        {
            return SourceType.FullName + "|" + PropertyName;
        }
        private bool IsCacheable()
        {
            return SourceType.IsClass;
        }
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if (IsCacheable()==false)
                return inner.TryResolve(context, destinationProperty);

            if(GetValue==null)
                throw new InvalidOperationException("GetValue was not assigned to this resolver.");
            return new Result(true,GetValue(context.Source));
        }
    }
}