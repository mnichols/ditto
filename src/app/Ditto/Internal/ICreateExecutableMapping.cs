using System;

namespace Ditto.Internal
{
    public interface ICreateExecutableMapping
    {
        IExecuteMapping CreateExecutableMapping(Type sourceType);
        Type DestinationType { get; }
    }
}