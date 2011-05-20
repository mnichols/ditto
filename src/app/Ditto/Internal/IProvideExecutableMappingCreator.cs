using System;

namespace Ditto.Internal
{
    public interface IProvideExecutableMappingCreator
    {
        ICreateExecutableMapping GetExecutableMappingCreator(Type destinationType);
    }
}