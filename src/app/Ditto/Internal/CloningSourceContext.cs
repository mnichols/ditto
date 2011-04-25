using System;

namespace Ditto.Internal
{
    public class CloningSourceContext : ISourceContext, ICacheable, IBindable,IValidatable
    {
        private readonly ISourceContext inner;

        public CloningSourceContext(ISourceContext inner)
        {
            this.inner = inner;
        }

        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            var container = inner as IContainResolvers;
            if (container == null)
                return false;
            return container.WillResolve(destinationProperty);
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            var container = inner as IContainResolvers;
            if (container == null)
                throw new DittoExecutionException("{0} is not a resolver container",inner);
            return container.GetResolver(destinationProperty);
        }


        public void Accept(IVisitCacheable visitor)
        {
            var cacheable = inner as ICacheable;
            if (cacheable == null)
                return;
            cacheable.Accept(visitor);
        }

        public MissingProperties Validate()
        {
            var validatable = inner as IValidatable;
            if (validatable == null)
                return MissingProperties.None;
            return validatable.Validate();
        }

        public void Assert()
        {
            var validatable = inner as IValidatable;
            if (validatable == null)
                return;
            validatable.Assert();
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var bindable = inner as IBindable;
            if (bindable == null)
                return;

            bindable.Bind(configurations);
        }

        public Type SourceType
        {
            get { return inner.SourceType; }
        }

        public void SetResolver(IDescribeMappableProperty destinationProperty, IResolveValue resolver)
        {
            inner.SetResolver(destinationProperty, resolver);
        }

        public SourcedConvention ApplyConvention(Convention convention)
        {
            return inner.ApplyConvention(convention);
        }

        public bool RequiresComponentConfigurationFor(IDescribeMappableProperty destinationProperty)
        {
            return inner.RequiresComponentConfigurationFor(destinationProperty);
        }

        public bool TrySetResolver(IDescribeMappableProperty destinationProperty, IResolveValue resolver)
        {
            return inner.TrySetResolver(destinationProperty, resolver);
        }

        public IDescribeMappableProperty CreateProperty(IDescribeMappableProperty destinationProperty)
        {
            return inner.CreateProperty(destinationProperty);
        }

        public bool RequiresCollectionConfigurationFor(IDescribeMappableProperty destinationProperty)
        {
            return inner.RequiresCollectionConfigurationFor(destinationProperty);
        }
    }
}