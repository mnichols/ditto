using System;

namespace Ditto.Internal
{
    public interface IDescribeMappableProperty
    {
        string Name { get; }
        Type PropertyType { get; }
        Type DeclaringType { get; }
        IDescribeMappableProperty ElementType { get; }
        IDescribePropertyElement ElementAt(int index);
        bool IsCustomType { get; }
        bool IsCollection { get; }
    }
}