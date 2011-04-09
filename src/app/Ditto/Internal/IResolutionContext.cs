using System;

namespace Ditto.Internal
{

    public interface IResolutionContext
    {
        Type SourceType { get; }
        object Source { get; }
        object Destination { get; }
        IAssignValue BuildValueAssignment(IDescribeMappableProperty destinationProperty);
        object GetSourcePropertyValue(string propertyName);
        object GetDestinationPropertyValue(string propertyName);
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty);
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty, IDescribeMappableProperty sourceProperty);
    }

}