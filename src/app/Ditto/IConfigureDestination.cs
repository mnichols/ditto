using System;
using Ditto.Internal;

namespace Ditto
{
    /// <summary>
    /// The primary interface for starting configuration on a destination type
    /// </summary>
    public interface IConfigureDestination:ITakeDestinationConfigurationSnapshot
    {
        /// <summary>
        /// Provide all the types which will provide data for this <c>destination</c> type.
        /// </summary>
        /// <param name="sourceTypes">The source types.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration From(params Type[] sourceTypes);
        /// <summary>
        /// Sets the property resolver based on the <paramref name="sourceType"/> being resolved from.
        /// </summary>
        /// <param name="destinationPropertyCriterion">The destination property criterion.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="resolver">The resolver.</param>
        void SetPropertyResolver(IPropertyCriterion destinationPropertyCriterion, Type sourceType,IResolveValue resolver);
        /// <summary>
        /// Apply the resolver according to the <paramref name="propertyCriterion"/> convention.
        /// </summary>
        /// <param name="propertyCriterion">The property criterion.</param>
        /// <param name="resolver">The resolver.</param>
        void ApplyingConvention(IPropertyCriterion propertyCriterion, IResolveValue resolver);

    }

    /// <summary>
    /// The primary interface for starting configuration on a destination of <typeparamref name="TDest"/>
    /// </summary>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public interface IConfigureDestination<TDest>:ITakeDestinationConfigurationSnapshot
    {
        /// <summary>
        /// Provide all the types which will provide data for this <c>destination</c> of <typeparamref name="TDest"/>.
        /// </summary>
        /// <param name="sourceTypes">The source types.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> From(params Type[] sourceTypes);
        /*convenience methods*/
        ISourcedDestinationConfiguration<TDest> From<TSource>();
        ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2>();
        ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2, TSource3>();
    }
}