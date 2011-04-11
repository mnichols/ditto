using System;

namespace Ditto.Internal
{
    public class SourcedConvention:IContainResolvers,ICacheable,IBindable
    {
        private Type sourceType;
        private readonly Convention inner;
        private readonly IResolveValue resolver;

        public SourcedConvention(Type sourceType,Convention inner,IResolveValue resolver)
        {
            this.sourceType = sourceType;
            this.inner = inner;
            this.resolver = resolver;
        }

        public Type SourceType{get {return sourceType;}}

        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            return inner.WillResolve(destinationProperty);
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            return inner.GetResolver(destinationProperty);
        }

        public void Accept(IVisitCacheable visitor)
        {
            var cachingResolver = new CachingPropertyNameResolver();
            cachingResolver.TryCache(sourceType,visitor,resolver);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var bindable = resolver as IBindable;
            if(bindable ==null)
                return;
            bindable.Bind(configurations);
        }
    }
}