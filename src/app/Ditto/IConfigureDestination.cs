using System;

namespace Ditto
{
    public interface IConfigureDestination
    {
        Type DestinationType { get; }
        ISourcedDestinationConfiguration From(params Type[] sourceTypes);

        void SetPropertyResolver(IPropertyCriterion destinationPropertyCriterion, Type sourceType,
                                 IResolveValue resolver);

        void ApplyingConvention(IPropertyCriterion propertyCriterion, IResolveValue resolver);
    }

    public interface IConfigureDestination<TDest>
    {
        ISourcedDestinationConfiguration<TDest> From(params Type[] sourceTypes);
        ISourcedDestinationConfiguration<TDest> From<TSource>();
        ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2>();
        ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2, TSource3>();
    }
}