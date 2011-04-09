using System;

namespace Ditto.Internal
{
    public interface ICreateMappingCommand
    {
        IMapCommand CreateCommand(Type sourceType, Type destinationType);
    }
}