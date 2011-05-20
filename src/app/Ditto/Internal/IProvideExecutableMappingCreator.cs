using System;

namespace Ditto.Internal
{
    public interface IProvideExecutableMappingCreator
    {
        ICreateExecutableMapping GetExecutableMapping(Type destinationType);
    }
}