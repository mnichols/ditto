using System;

namespace Ditto.Internal
{
    public interface IActivate
    {
        object CreateInstance(Type ofType);
        object CreateCollectionInstance(Type collectionType, int length);
    }

    
}