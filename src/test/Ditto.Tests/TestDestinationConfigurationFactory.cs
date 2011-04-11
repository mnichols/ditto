using System;
using System.Linq;
using Ditto.Internal;
using log4net.Repository.Hierarchy;

namespace Ditto.Tests
{

    public class TestDestinationConfigurationFactory:ICreateDestinationConfiguration,ICreateBindableConfiguration
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
            return new BindableConfiguration(snapshot.DestinationType, snapshot.DestinationProperties, snapshot.SourceContexts.ToArray(), snapshot.Conventions.ToArray(), null);
        }

        public BindableConfiguration CreateBindableConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}