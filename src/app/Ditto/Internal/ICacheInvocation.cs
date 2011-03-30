using System;

namespace Ditto.Internal
{
    public interface ICacheInvocation
    {
        GetValue CacheGet(Type targetType, string propertyName);
        SetValue CacheSet(Type targetType, string propertyName);
    }
}