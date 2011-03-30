using System;

namespace Ditto.Internal
{
    public interface ICreateBindableConfiguration
    {
        BindableConfiguration CreateBindableConfiguration();
        Type DestinationType { get; }
    }
}