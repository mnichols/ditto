using System;

namespace Ditto.Internal
{
    public class NullLogFactory : ILogFactory
    {
        public ILog Create(Type type)
        {
            return new NullLogger();
        }

        public ILog Create(object instance)
        {
            return new NullLogger();
        }
    }
}