using System;

namespace Ditto.Internal
{
    public class SourcedConvention:IResolverContainer,ICacheable,IBindable
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

        public bool WillResolve(IDescribeMappableProperty mappableProperty)
        {
            return inner.WillResolve(mappableProperty);
        }

        public IResolveValue GetResolver(IDescribeMappableProperty mappableProperty)
        {
            return inner.GetResolver(mappableProperty);
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