using System;

namespace Ditto.Internal
{
    public interface ICreateDestinationConfiguration
    {
        IConfigureDestination Create(Type destinationType);
        IConfigureDestination<TDest> Create<TDest>();
    }
}