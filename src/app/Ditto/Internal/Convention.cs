using System;
using System.Linq;

namespace Ditto.Internal
{
    public class Convention : IContainResolvers
    {
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly IResolveValue resolver;

        public Convention(IDescribeMappableProperty[] destinationProperties, IResolveValue resolver)
        {
            this.destinationProperties = destinationProperties;
            this.resolver = resolver;
        }

        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            return destinationProperties.Any(are => string.Equals(are.Name,destinationProperty.Name));
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            if (WillResolve(destinationProperty) == false)
                throw new InvalidOperationException("Resolver not configured for " + destinationProperty);
            return resolver;
        }

        public bool HasResolverFromOtherSource(Type destinationType, IDescribeMappableProperty destinationProperty)
        {
            //really this shouldn't matter
            return false;
        }

        public SourcedConvention Apply(Type sourceType)
        {
            if(sourceType==null)
                throw new ArgumentNullException("sourceType");
            return new SourcedConvention(sourceType,this,resolver);
        }
    }
}