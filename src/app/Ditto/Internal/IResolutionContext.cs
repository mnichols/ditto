using System;

namespace Ditto.Internal
{

    public interface IResolutionContext
    {
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty);
        Type SourceType { get; }
        object Source { get; }
        object Destination { get; }
        IAssignValue Scope(IDescribeMappableProperty destinationProperty);
        object GetSourcePropertyValue(string propertyName);

        object GetDestinationPropertyValue(string propertyName);
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty, IDescribeMappableProperty sourceProperty);
    }
}