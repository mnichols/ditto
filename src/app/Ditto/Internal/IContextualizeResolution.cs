using System;

namespace Ditto.Internal
{
    public interface IContextualizeResolution
    {
        IResolutionContext CreateContext(object source, object destination);
        IResolutionContext CreateContext(object source, Type destinationType);

        /// <summary>
        /// Creates a nested context from current context, nesting both Source and Destination properties in parallel.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="destinationProperty">The destination property.</param>
        /// <param name="parentContext">The parent context.</param>
        /// <returns></returns>
        IResolutionContext CreateNestedParallelContext(IDescribeMappableProperty sourceProperty,IDescribeMappableProperty destinationProperty,IResolutionContext parentContext) ;
        /// <summary>
        /// Creates a nested context from current context, but using the current Source.
        /// </summary>
        /// <param name="destinationProperty">The destination property.</param>
        /// <param name="parentContext">The parent context.</param>
        /// <returns></returns>
        IResolutionContext CreateNestedContext(IDescribeMappableProperty destinationProperty, IResolutionContext parentContext);
    }
}