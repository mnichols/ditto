using System;

namespace Ditto.Internal
{
    public interface ICreateMappingCommand
    {
        IMapCommand CreateCommand(Type destinationType, Type sourceType);
    }
}