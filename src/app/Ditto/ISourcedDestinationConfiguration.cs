using System;
using System.Linq.Expressions;
using Ditto.Internal;

namespace Ditto
{
    /// <summary>
    /// Configuration based on a specific source type
    /// </summary>
    public interface ISourcedDestinationConfiguration
    {
        /// <summary>
        /// Applies convention regardless of the source type being used in execution.
        /// </summary>
        /// <param name="destinationPropertyCriterion">The destination property criterion.</param>
        /// <param name="resolver">The resolver.</param>
        void ApplyingConvention(IPropertyCriterion destinationPropertyCriterion, IResolveValue resolver);
    }
    /// <summary>
    /// Configuration for <typeparamref name="TDest"/> based on a specific source type
    /// </summary>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public interface ISourcedDestinationConfiguration<TDest>
    {
        /// <summary>
        /// Redirects <paramref name="sourceProperty"/> value to <paramref name="destinationProperty"/>. 
        /// This works for component (custom type) properties, too.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="destinationProperty">The destination property.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Redirecting<TSource>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty);


        /// <summary>
        /// Redirects <paramref name="sourceProperty"/> value to <paramref name="destinationProperty"/> where each are different types and you want to provide a nested config.
        /// This nested config is also validated by <c>ditto</c>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TNest">The type of the nest.</typeparam>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="destinationProperty">The destination property.</param>
        /// <param name="nestedCfg">The nested CFG.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Redirecting<TSource, TNest>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> nestedCfg);

        /// <summary>
        /// Simply provide static value to be used each time for any of the <paramref name="destinationProperties"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="destinationProperties">The destination properties.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> UsingValue<TSource>(object value, params Expression<Func<TDest, object>>[] destinationProperties);

        /// <summary>
        /// Applies convention regardless of the source type being used in execution.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="destinationProperties">The destination properties.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> ApplyingConvention(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties);

        /// <summary>
        /// Toggles <c>bool</c> properties <c>true</c>/<c>false</c> based on events being resolved.
        /// </summary>
        /// <param name="destinationProperty">The destination property.</param>
        /// <param name="toggle">The toggle.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Toggling(Expression<Func<TDest, object>> destinationProperty, Action<IToggle<TDest>> toggle);

        /// <summary>
        /// Transforms the full source declaring object into the <paramref name="destinationProperties"/>. 
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destinationProperties">The destination properties.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Transforming<TSource>(Func<TSource, object> source, params Expression<Func<TDest, object>>[] destinationProperties);

        /// <summary>
        /// Provide your own <paramref name="resolver"/> for these <paramref name="destinationProperties"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="destinationProperties">The destination properties.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Resolving<TSource>(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties);


        /// <summary>
        /// Provide another configuration for a nested component. This is not necesary for you to do if you have already configure your nested component elsewhere since everything is bound together
        /// before validation and execution. This is typically used for 'unflattening' a source object into a nested component on the destination.
        /// TODO : This behavior needs to be confirmed that it is not overriden by framework
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TNest">The type of the nest.</typeparam>
        /// <param name="destinationProperty">The destination property.</param>
        /// <param name="sourceConfig">The source config.</param>
        /// <returns></returns>
        ISourcedDestinationConfiguration<TDest> Nesting<TSource, TNest>(Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> sourceConfig);
    }
}