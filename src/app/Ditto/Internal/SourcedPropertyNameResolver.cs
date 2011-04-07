using System;

namespace Ditto.Internal
{
    public class SourcedPropertyNameResolver:ICacheable,IResolveValue
    {
        internal Type SourceType { get; private set; }
        internal string PropertyName { get; private set; }

        internal GetValue GetValue { get; set; }

        public SourcedPropertyNameResolver(Type sourceType, string propertyName)
        {
            SourceType = sourceType;
            PropertyName = propertyName;
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
            return new Result(true,GetValue(context.Source));
        }
    }
}