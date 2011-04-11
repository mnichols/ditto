using System;
using Ditto.Internal;

namespace Ditto.Tests
{
    public class TestConfigurationFactory : ICreateDestinationConfiguration
    {
        public IConfigureDestination Create(Type destinationType)
        {
            return new DestinationConfiguration(destinationType, this);
        }

        public IConfigureDestination<TDest> Create<TDest>()
        {
            return new DestinationConfiguration<TDest>(this);
        }

        public BindableConfiguration CreateBindableConfiguration(DestinationConfigurationMemento snapshot)
        {
            return new BindableConfiguration(snapshot);
        }
    }
}