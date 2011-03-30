using System;

namespace Ditto
{
    public interface ICreateLogger
    {
        ILog Create(Type type);
    }
}