using System;
using System.Collections.Generic;
using Ditto.Internal;

namespace Ditto
{
    public interface IContainDestinationConfiguration
    {
        IConfigureDestination Map(Type destinationType);
        IConfigureDestination<TDest> Map<TDest>();
        bool HasConfigurationFor(Type destinationType);
        ICollection<BindableConfiguration> CreateBindableConfigurations();
    }
}