using System;
using System.Linq;

namespace Ditto.Internal
{
    public class ConfigurationValidator : IValidatable
    {
        private readonly Type destinationType;
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly IContainResolvers[] resolverContainers;
        private MissingProperties missingProperties;

        public ConfigurationValidator(Type destinationType, IDescribeMappableProperty[] destinationProperties,
                                      IContainResolvers[] resolverContainers)
        {
            if (destinationProperties.Length == 0)
                throw new DittoConfigurationException("Destination properties have not been queried for {0}",destinationType);
            missingProperties = new MissingProperties();
            this.destinationType = destinationType;
            this.destinationProperties = destinationProperties;
            this.resolverContainers = resolverContainers;
        }

        public MissingProperties Validate()
        {
            var resolverValidatable = resolverContainers.OfType<IValidateResolvers>().ToArray();
            if(resolverValidatable.Length>0)
            {
                foreach (var destinationProperty in destinationProperties)
                {
                    var property = destinationProperty;
                    if (resolverValidatable.Any(it => it.HasResolverFromOtherSource(destinationType,property)))
                        continue;
                    missingProperties.Add(destinationProperty);
                }
            }
            foreach (var validatable in resolverContainers.OfType<IValidatable>())
            {
                missingProperties = missingProperties.Merge(validatable.Validate());
            }
            return missingProperties;
        }

        public void Assert()
        {
            var props = Validate();
            props.TryThrow();
        }
    }
}