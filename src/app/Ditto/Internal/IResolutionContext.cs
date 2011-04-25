using System;

namespace Ditto.Internal
{

    /// <summary>
    /// 
    /// </summary>
    public interface IResolutionContext
    {
        /// <summary>
        /// Gets the type of the source for current resolution.
        /// </summary>
        /// <value>The type of the source.</value>
        Type SourceType { get; }
        /// <summary>
        /// Gets the declaring source object (not the property value).
        /// This can be <c>null</c>. Be aware of this when implementing <see cref="IResolveValue"/>.
        /// </summary>
        /// <value>The source.</value>
        object Source { get; }
        /// <summary>
        /// Gets the destination. This is the declaring object for the resulting value, not the destination property itself.
        /// </summary>
        /// <value>The destination.</value>
        object Destination { get; }

        /// <summary>
        /// Used by <c>ditto </c> to prepare the destination property for value assignment during execution.
        /// </summary>
        /// <param name="destinationProperty">The destination property.</param>
        /// <returns></returns>
        IAssignValue BuildValueAssignment(IDescribeMappableProperty destinationProperty);
        object GetSourcePropertyValue(string propertyName);
        object GetDestinationPropertyValue(string propertyName);
        /// <summary>
        /// Nests this context only on the provided <paramref name="destinationProperty"/>. The source declaring object remains the same in the resulting context.
        /// </summary>
        /// <param name="destinationProperty">The destination property.</param>
        /// <returns></returns>
        IResolutionContext Nested(IDescribeMappableProperty destinationProperty);

        /// <summary>
        /// Nests this context on both the destination and source properties. 
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="destinationProperty">The destination property.</param>
        /// <returns></returns>
        IResolutionContext Nested(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty);
    }

}