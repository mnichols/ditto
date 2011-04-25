using System;

namespace Ditto.Internal
{
    public class DestinationConfigurationMemento
    {
        public Type DestinationType { get; private set; }
        public IDescribeMappableProperty[] DestinationProperties { get; private set; }
        public ISourceContext[] SourceContexts { get; private set; }
        public Convention[] Conventions { get; private set; }

        public DestinationConfigurationMemento(Type destinationType, IDescribeMappableProperty[] destinationProperties, ISourceContext[] sourceContexts, Convention[] conventions)
        {
            DestinationType = destinationType;
            DestinationProperties = destinationProperties;
            SourceContexts = sourceContexts;
            Conventions = conventions;
        }
        public override string ToString()
        {
            return GetType() + " for " + DestinationType;
        }
    }
}