using System;
using Ditto.Internal;

namespace Ditto.Tests
{
    public class TestDestinationConfigurationFactory:ICreateDestinationConfiguration
    {
        public IConfigureDestination Create(Type destinationType)
        {
            return new DestinationConfiguration(destinationType);

        }

        public IConfigureDestination<TDest> Create<TDest>()
        {
            return new DestinationConfiguration<TDest>(this);
        }
    }
}