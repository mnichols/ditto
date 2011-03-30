using System;

namespace Ditto.Internal
{
    public interface IActivator
    {
        object CreateInstance(Type ofType);
        object CreateCollectionInstance(Type collectionType, int length);
    }

    
}