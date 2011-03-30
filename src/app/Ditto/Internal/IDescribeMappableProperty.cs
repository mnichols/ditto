using System;

namespace Ditto.Internal
{
    public interface IDescribeMappableProperty
    {
        string Name { get; }
        Type PropertyType { get; }
        Type DeclaringType { get; }
        IDescribePropertyElement ElementAt(int index);
    }
}