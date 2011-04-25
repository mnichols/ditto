using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ditto.Criteria;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class DestinationConfiguration<TDest> : IConfigureDestination<TDest>, IApplyConventions,
                                                   ISourcedDestinationConfiguration<TDest>
    {
        private readonly ICreateDestinationConfiguration configurations;
        private readonly IConfigureDestination inner;

        public DestinationConfiguration(ICreateDestinationConfiguration configurations)
        {
            this.configurations = configurations;
            inner = configurations.Create(typeof (TDest));
        }

        public void Apply(IPropertyCriterion propertyCriterion, IResolveValue resolver)
        {
            inner.ApplyingConvention(propertyCriterion, resolver);
        }
        public ISourcedDestinationConfiguration<TDest> ForCloningOnly()
        {
            inner.ForCloningOnly();
            return this;
        }
        public ISourcedDestinationConfiguration<TDest> From(params Type[] sourceTypes)
        {
            inner.From(sourceTypes);
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> From<TSource>()
        {
            inner.From(typeof (TSource));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2>()
        {
            inner.From(typeof (TSource1), typeof (TSource2));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2, TSource3>()
        {
            inner.From(typeof (TSource1), typeof (TSource2), typeof (TSource3));
            return this;
        }

        public DestinationConfigurationMemento TakeSnapshot()
        {
            var snapshot = inner as ITakeDestinationConfigurationSnapshot;
            if (snapshot == null)
                return null;
            return snapshot.TakeSnapshot();
        }

        public ISourcedDestinationConfiguration<TDest> Toggling(Expression<Func<TDest, object>> destinationProperty,
                                                                Action<IToggle<TDest>> toggle)
        {
            var toggler = new Toggler<TDest>(this, destinationProperty);
            toggle(toggler);
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> Transforming<TSource>(Func<TSource, object> source,
                                                                             params Expression<Func<TDest, object>>[]
                                                                                 destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var destinationProperty in destinationProperties)
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty), typeof (TSource),
                                          new LambdaResolver<TSource>(source));
            }

            return this;
        }

        public ISourcedDestinationConfiguration<TDest> Redirecting<TSource>(
            Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty)
        {
            var mappableDestinationProperty = MappableProperty.For(destinationProperty);
            var mappableSourceProperty = MappableProperty.For(sourceProperty);
            if(mappableDestinationProperty.IsCustomType)
            {
                var nestedConfig = configurations.Create(mappableDestinationProperty.PropertyType);
                nestedConfig.From(mappableSourceProperty.PropertyType);
                inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),
                                      typeof(TSource),
                                      new RedirectingConfigurationResolver(MappableProperty.For(sourceProperty),
                                          configurations.CreateBindableConfiguration(nestedConfig.TakeSnapshot())));
            }
            else
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty), typeof(TSource),
                                          new PropertyNameResolver(Reflect.GetProperty(sourceProperty).Name));    
            }
            
            return this;
        }
        public ISourcedDestinationConfiguration<TDest>  Redirecting<TSource, TNest>(
            Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty,
            Action<ISourcedDestinationConfiguration<TNest>> nestedCfg)
        {
            var mappableSourceProperty = MappableProperty.For(sourceProperty);
            var newNestedConfig = configurations.Create<TNest>();
            var sourced = newNestedConfig.From(mappableSourceProperty.PropertyType);
            nestedCfg(sourced);
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),
                                      typeof (TSource),
                                      new RedirectingConfigurationResolver(mappableSourceProperty,
                                          configurations.CreateBindableConfiguration(newNestedConfig.TakeSnapshot())));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> Resolving<TSource>(IResolveValue resolver,
                                                                          params Expression<Func<TDest, object>>[]
                                                                              destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(expression), typeof (TSource), resolver);
            }

            return this;
        }


        public ISourcedDestinationConfiguration<TDest> Nesting<TSource, TNest>(
            Expression<Func<TDest, object>> destinationProperty,
            Action<ISourcedDestinationConfiguration<TNest>> sourceConfig)
        {
            var nestedConfig = configurations.Create<TNest>();
            var sourced = nestedConfig.From<TSource>();
            sourceConfig(sourced);
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),
                                      typeof (TSource),
                                      new NestingConfigurationResolver(
                                          configurations.CreateBindableConfiguration(nestedConfig.TakeSnapshot())));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> UsingValue<TSource>(object value,
                                                                           params Expression<Func<TDest, object>>[]
                                                                               destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(expression), typeof (TSource),
                                          new StaticValueResolver(value));
            }

            return this;
        }

        public ISourcedDestinationConfiguration<TDest> ApplyingConvention(IResolveValue resolver,
                                                                          params Expression<Func<TDest, object>>[]
                                                                              destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.ApplyingConvention(PropertyNameCriterion.From(expression), resolver);
            }

            return this;
        }

        private void AssertPropertiesProvided(params Expression<Func<TDest, object>>[] destinationProperties)
        {
            if (destinationProperties == null || destinationProperties.Length == 0)
                throw new DittoConfigurationException("At least one destination property must be specified");
        }
    }

    public class DestinationConfiguration : IConfigureDestination, IApplyConventions, ISourcedDestinationConfiguration
    {
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly List<Convention> conventions = new List<Convention>();
        private readonly Type destinationType;
        private readonly List<ISourceContext> sourceContexts = new List<ISourceContext>();

        public DestinationConfiguration(Type destinationType)
        {
            this.destinationType = destinationType;
            destinationProperties =destinationType.GetProperties().Select(into => new MappableProperty(into)).ToArray();
            Logger = new NullLogFactory();
            MakeSelfAware();
        }

        private void MakeSelfAware()
        {
            From(destinationType);
        }


        public ILogFactory Logger { get; set; }

        public void Apply(IPropertyCriterion propertyCriterion, IResolveValue resolver)
        {
            ApplyingConvention(propertyCriterion, resolver);
        }
        public bool IsSourcedBy(Type sourceType)
        {
            return sourceContexts.Any(its => its.SourceType == sourceType);
        }
        public ISourcedDestinationConfiguration ForCloningOnly()
        {
            Logger.Create(this).Debug("Mapping '{0}' for cloning.");
            var toWrap = sourceContexts.First(its => its.SourceType == destinationType);
            sourceContexts.RemoveAll(its => its.SourceType == destinationType);
            sourceContexts.Add(new CloningSourceContext(toWrap));
            return this;
        }
        public ISourcedDestinationConfiguration From(params Type[] sourceTypes)
        {
            if (sourceTypes == null || sourceTypes.Length == 0)
                throw new DittoConfigurationException("At least one source type must be specified.");
            sourceTypes.ForEach(src => Logger.Create(this).Debug("Mapping '{0}' from '{1}'", destinationType, src));
            foreach (var sourceType in sourceTypes)
            {
                var source = new SourceContext(sourceType);
                source.SetDestinationContext(destinationProperties);
                sourceContexts.Add(source);
            }

            return this;
        }


        public void ApplyingConvention(IPropertyCriterion destinationPropertyCriterion, IResolveValue resolver)
        {
            var matches = destinationProperties.Where(destinationPropertyCriterion.IsSatisfiedBy).ToArray();
            var convention = new Convention(matches, resolver);
            conventions.Add(convention);
        }


        public void SetPropertyResolver(IPropertyCriterion destinationPropertyCriterion, Type sourceType,
                                        IResolveValue resolver)
        {
            var ctx = DemandSourceContext(sourceType);
            var prop = DemandDestinationProperty(destinationPropertyCriterion);
            ctx.SetResolver(prop, resolver);
        }

        public DestinationConfigurationMemento TakeSnapshot()
        {
            return new DestinationConfigurationMemento(destinationType, destinationProperties, sourceContexts.ToArray(),
                                                       conventions.ToArray());
        }

        private IDescribeMappableProperty DemandDestinationProperty(IPropertyCriterion propertyCriterion)
        {
            if (propertyCriterion == null)
                throw new ArgumentNullException("propertyCriterion");
            var prop = destinationProperties.FirstOrDefault(propertyCriterion.IsSatisfiedBy);
            if (prop != null)
                return prop;
            throw new InvalidOperationException(propertyCriterion + " is not valid for " + destinationType);
        }

        private ISourceContext DemandSourceContext(Type sourceType)
        {
            if (sourceContexts.Count == 0)
                throw new DittoConfigurationException(
                    "Sources have not been setup for '{0}'. Did you forget to call 'From'?", destinationType);
            var ctx = sourceContexts.FirstOrDefault(its => its.SourceType == sourceType);
            if (ctx != null)
                return ctx;
            throw new DittoConfigurationException("'{0}' has not been setup as a source for '{1}'.", sourceType,
                                                    destinationType);
        }
    }
}