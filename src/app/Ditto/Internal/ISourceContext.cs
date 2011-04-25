using System;

namespace Ditto.Internal
{
    public interface ISourceContext:IContainResolvers
    {
        Type SourceType { get; }
        void SetResolver(IDescribeMappableProperty destinationProperty, IResolveValue resolver);
        SourcedConvention ApplyConvention(Convention convention);
        bool RequiresComponentConfigurationFor(IDescribeMappableProperty destinationProperty);
        bool TrySetResolver(IDescribeMappableProperty destinationProperty,IResolveValue resolver);
        IDescribeMappableProperty CreateProperty(IDescribeMappableProperty destinationProperty);
        bool RequiresCollectionConfigurationFor(IDescribeMappableProperty destinationProperty);
    }
}