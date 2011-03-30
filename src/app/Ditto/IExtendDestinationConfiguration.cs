using System;

namespace Ditto
{
    public interface IExtendDestinationConfiguration
    {
        Type DestinationType { get; }
    }
}