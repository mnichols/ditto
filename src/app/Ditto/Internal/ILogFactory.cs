using System;

namespace Ditto.Internal
{
    public interface ILogFactory
    {
        ILog Create(Type type);
        ILog Create(object instance);
    }
}