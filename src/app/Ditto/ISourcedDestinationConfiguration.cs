using System;
using System.Linq.Expressions;
using Ditto.Internal;

namespace Ditto
{
    public interface ISourcedDestinationConfiguration
    {
        void ApplyingConvention(IPropertyCriterion destinationPropertyCriterion, IResolveValue resolver);
    }
    public interface ISourcedDestinationConfiguration<TDest>
    {
        ISourcedDestinationConfiguration<TDest> Redirecting<TSource>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty);

        ISourcedDestinationConfiguration<TDest> Redirecting<TSource, TNest>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> nestedCfg);

        ISourcedDestinationConfiguration<TDest> UsingValue<TSource>(object value, params Expression<Func<TDest, object>>[] destinationProperties);

        ISourcedDestinationConfiguration<TDest> ApplyingConvention(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties);

        ISourcedDestinationConfiguration<TDest> Toggling(Expression<Func<TDest, object>> destinationProperty, Action<IToggle<TDest>> toggle);

        ISourcedDestinationConfiguration<TDest> Transforming<TSource>(Func<TSource, object> source, params Expression<Func<TDest, object>>[] destinationProperties);

        ISourcedDestinationConfiguration<TDest> Resolving<TSource>(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties);


        ISourcedDestinationConfiguration<TDest> Nesting<TSource, TNest>(Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> sourceConfig);
    }
}