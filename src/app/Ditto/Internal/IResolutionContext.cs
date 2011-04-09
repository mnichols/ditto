using System;

namespace Ditto.Internal
{

    public interface IResolutionContext
    {
        Type SourceType { get; }
        object Source { get; }
        object Destination { get; }
        IAssignValue Scope(IDescribeMappableProperty destinationProperty);
        object GetSourcePropertyValue(string propertyName);
        object GetDestinationPropertyValue(string propertyName);
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty);
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty, IDescribeMappableProperty sourceProperty);
    }

}