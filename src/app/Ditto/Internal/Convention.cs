using System;
using System.Linq;

namespace Ditto.Internal
{
    public class Convention : IResolverContainer
    {
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly IResolveValue resolver;

        public Convention(IDescribeMappableProperty[] destinationProperties, IResolveValue resolver)
        {
            this.destinationProperties = destinationProperties;
            this.resolver = resolver;
        }

        public bool WillResolve(IDescribeMappableProperty mappableProperty)
        {
            return destinationProperties.Any(are => string.Equals(are.Name,mappableProperty.Name));
        }

        public IResolveValue GetResolver(IDescribeMappableProperty mappableProperty)
        {
            if (WillResolve(mappableProperty) == false)
                throw new InvalidOperationException("Resolver not configured for " + mappableProperty);
            return resolver;
        }
        public SourcedConvention Apply(Type sourceType)
        {
            if(sourceType==null)
                throw new ArgumentNullException("sourceType");
            return new SourcedConvention(sourceType,this,resolver);
        }
    }
}