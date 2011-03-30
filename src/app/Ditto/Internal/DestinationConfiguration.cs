﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ditto.Criteria;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class DestinationConfiguration<TDest> : IValidatable, IConfigureDestination<TDest>,ICreateExecutableMapping,ICacheable,IApplyConventions,IBindable,ISourcedDestinationConfiguration<TDest>,ICreateBindableConfiguration
    {
        private readonly ICreateDestinationConfiguration configurations;
        private readonly IConfigureDestination inner;

        public DestinationConfiguration(ICreateDestinationConfiguration configurations)
        {
            this.configurations = configurations;
            inner = configurations.Create(typeof(TDest));
        }
        public Type DestinationType { get { return inner.DestinationType; } }
        public ISourcedDestinationConfiguration<TDest> Toggling(Expression<Func<TDest, object>> destinationProperty, Action<IToggle<TDest>> toggle)
        {
            var toggler = new Toggler<TDest>(this,destinationProperty);
            toggle(toggler);
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> Transforming<TSource>(Func<TSource, object> source, Expression<Func<TDest, object>> destinationProperty)
        {
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),typeof(TSource),new LambdaResolver<TSource>(source));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> Redirecting<TSource>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty)
        {
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty), typeof (TSource),
                                      new PropertyNameResolver(Reflect.GetProperty(sourceProperty).Name));
            return this;
        }
        public ISourcedDestinationConfiguration<TDest> Redirecting<TSource, TNest>(Expression<Func<TSource, object>> sourceProperty, Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> nestedCfg)
        {
            var newNestedConfig = configurations.Create<TNest>().From<TSource>();
            nestedCfg(newNestedConfig);
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),
                                      typeof(TSource),
                                      new RedirectingConfigurationResolver(MappableProperty.For(sourceProperty), MappableProperty.For(destinationProperty), (ICreateExecutableMapping)newNestedConfig));
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
        public ISourcedDestinationConfiguration<TDest> From<TSource1,TSource2>()
        {
            inner.From(typeof(TSource1),typeof(TSource2));
            return this;
        }
        public ISourcedDestinationConfiguration<TDest> From<TSource1, TSource2,TSource3>()
        {
            inner.From(typeof(TSource1), typeof(TSource2),typeof(TSource3));
            return this;
        }
        private void AssertPropertiesProvided(params Expression<Func<TDest,object>>[] destinationProperties)
        {
            if(destinationProperties==null || destinationProperties.Length==0)
                throw new MappingConfigurationException("At least one destination property must be specified");
        }
        public ISourcedDestinationConfiguration<TDest> Resolving<TSource>(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(expression), typeof(TSource), resolver);
            }

            return this;
        }

        

        public ISourcedDestinationConfiguration<TDest> Nesting<TSource,TNest>(Expression<Func<TDest, object>> destinationProperty, Action<ISourcedDestinationConfiguration<TNest>> sourceConfig)
        {
            var nestedConfig = configurations.Create<TNest>().From<TSource>();
            sourceConfig(nestedConfig);
            inner.SetPropertyResolver(PropertyNameCriterion.From(destinationProperty),
                typeof (TSource),
                new NestingConfigurationResolver(MappableProperty.For(destinationProperty),(ICreateExecutableMapping)nestedConfig));
            return this;
        }

        public ISourcedDestinationConfiguration<TDest> UsingValue<TSource>(object value, params Expression<Func<TDest, object>>[] destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.SetPropertyResolver(PropertyNameCriterion.From(expression), typeof(TSource), new StaticValueResolver(value));    
            }
            
            return this;
        }

        public MissingProperties Validate()
        {
            var validatable = inner as IValidatable;
            if(validatable==null)
                return new MissingProperties();
            return validatable.Validate();
        }

        
        public IExecuteMapping CreateExecutableMapping(Type sourceType)
        {
            return ((ICreateExecutableMapping)inner).CreateExecutableMapping(sourceType);
        }

        public void Assert()
        {
            var validatable = inner as IValidatable;
            if (validatable == null)
                return;
            validatable.Assert();
        }

        public ISourcedDestinationConfiguration<TDest> ApplyingConvention(IResolveValue resolver, params Expression<Func<TDest, object>>[] destinationProperties)
        {
            AssertPropertiesProvided(destinationProperties);
            foreach (var expression in destinationProperties)
            {
                inner.ApplyingConvention(PropertyNameCriterion.From(expression), resolver);
            }

            return this;
            
        }


        public void Accept(IVisitCacheable visitor)
        {
            var cacheable = inner as ICacheable;
            if(cacheable==null)
                return;
            cacheable.Accept(visitor);
        }

        public void Apply(IPropertyCriterion propertyCriterion, IResolveValue resolver)
        {
            inner.ApplyingConvention(propertyCriterion, resolver);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var bindable = inner as IBindable;
            if (bindable == null)
                return;
           
            bindable.Bind(configurations);
        }

        public BindableConfiguration CreateBindableConfiguration()
        {
            var creator = inner as ICreateBindableConfiguration;
            if (creator == null)
                return null;
            return creator.CreateBindableConfiguration();
        }
    }

    public class DestinationConfiguration : IValidatable, IConfigureDestination,ICreateExecutableMapping,ICacheable,IBindable,IApplyConventions,ISourcedDestinationConfiguration,ICreateBindableConfiguration
    {
        private readonly IDescribeMappableProperty[] cachedDestinationProps;
        private readonly List<Convention> conventions = new List<Convention>();
        private readonly Type destinationType;
        private readonly IMapCommandFactory mapCommands;
        private readonly List<SourceContext> sourceContexts = new List<SourceContext>();
        private BindableConfiguration bindableConfiguration;

        public DestinationConfiguration(Type destinationType)
        {
            this.destinationType = destinationType;
            this.mapCommands = mapCommands;
            cachedDestinationProps = destinationType.GetProperties().Select(into => new MappableProperty(into)).ToArray();
            Logger = new NullLogFactory();
        }
        public BindableConfiguration CreateBindableConfiguration()
        {
            return new BindableConfiguration(this.destinationType,this.cachedDestinationProps,this.sourceContexts.ToArray(),conventions.ToArray()){Logger = Logger};
        }
        public ILogFactory Logger { get; set; }
        public ISourcedDestinationConfiguration From(params Type[] sourceTypes)
        {
            if(sourceTypes==null || sourceTypes.Length==0)
                throw new MappingConfigurationException("At least one source type must be specified.");
            sourceTypes.ForEach(src=>Logger.Create(this).Debug("Mapping '{0}' from '{1}'",destinationType,src));
            foreach (var sourceType in sourceTypes)
            {
                var source = new SourceContext(sourceType);
                source.SetDestinationContext(cachedDestinationProps);
                sourceContexts.Add(source);
            }

            return this;
        }
        
        
        
        public IExecuteMapping CreateExecutableMapping(Type sourceType)
        {
            AssertExecutable();
            return bindableConfiguration.CreateExecutableMapping(sourceType);
        }
        public MissingProperties Validate()
        {
            var validator = new ConfigurationValidator(destinationType,
                cachedDestinationProps,
                sourceContexts.Concat<IResolverContainer>(conventions).ToArray());
            return validator.Validate();
        }

        public void Assert()
        {
            var missing = Validate();
            missing.TryThrow();
        }

        public void ApplyingConvention(IPropertyCriterion destinationPropertyCriterion, IResolveValue resolver)
        {
            var convention = new Convention(Match(destinationPropertyCriterion), resolver);
            conventions.Add(convention);
        }

        

        public Type DestinationType
        {
            get { return destinationType; }
        }

        private IDescribeMappableProperty[] Match(IPropertyCriterion propertyCriterion)
        {
            return cachedDestinationProps.Where(propertyCriterion.IsSatisfiedBy).ToArray();
        }

        private IDescribeMappableProperty DemandDestinationProperty(IPropertyCriterion propertyCriterion)
        {
            if (propertyCriterion == null)
                throw new ArgumentNullException("propertyCriterion");
            var prop = cachedDestinationProps.FirstOrDefault(propertyCriterion.IsSatisfiedBy);
            if (prop != null)
                return prop;
            throw new InvalidOperationException(propertyCriterion + " is not valid for " + destinationType);
        }

        public void SetPropertyResolver(IPropertyCriterion destinationPropertyCriterion, Type sourceType,IResolveValue resolver)
        {
            var ctx = DemandSourceContext(sourceType);
            var prop = DemandDestinationProperty(destinationPropertyCriterion);
            ctx.SetResolver(prop, resolver);
        }
        
        private SourceContext DemandSourceContext(Type sourceType)
        {
            if(sourceContexts.Count==0)
                throw new MappingConfigurationException("Sources have not been setup for '{0}'. Did you forget to call 'From'?",destinationType);
            var ctx = sourceContexts.FirstOrDefault(its => its.SourceType == sourceType);
            if (ctx != null)
                return ctx;
            throw new MappingConfigurationException("'{0}' has not been setup as a source for '{1}'.", sourceType, destinationType);
        }
        public void Accept(IVisitCacheable visitor)
        {
            foreach (var sourceContext in sourceContexts)
            {
                sourceContext.Accept(visitor);
            }
            foreach (var cachedDestinationProp in cachedDestinationProps.OfType<ICacheable>())
            {
                cachedDestinationProp.Accept(visitor);
            }
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            bindableConfiguration = CreateBindableConfiguration();
            bindableConfiguration.Bind(configurations);
        }
        
        public void Apply(IPropertyCriterion propertyCriterion, IResolveValue resolver)
        {
            ApplyingConvention(propertyCriterion, resolver);
        }
        private void AssertExecutable()
        {
            if(bindableConfiguration==null)
            {
                throw new MappingExecutionException("Configuration for destination type '{0}' is not executable. This probably means 'Bind' was not called on the configuration.",destinationType);
            }
        }
    }
}