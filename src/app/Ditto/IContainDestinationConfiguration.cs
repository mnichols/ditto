using System;

namespace Ditto
{
    public interface IContainDestinationConfiguration
    {
        IConfigureDestination Map(Type destinationType);
        IConfigureDestination<TDest> Map<TDest>();
        bool HasConfigurationFor(Type destinationType);
    }
}