using System;

namespace Ditto.Internal
{
    public interface ICreateDestinationConfiguration:ICreateBindableConfiguration
    {
        IConfigureDestination Create(Type destinationType);
        IConfigureDestination<TDest> Create<TDest>();
    }
}