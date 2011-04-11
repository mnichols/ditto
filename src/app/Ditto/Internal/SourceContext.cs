using System;
using System.Collections.Generic;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class SourceContext : IContainResolvers,ICacheable,IValidatable,IBindable
    {
        private readonly Dictionary<IDescribeMappableProperty, IResolveValue> prop2Resolver = new Dictionary<IDescribeMappableProperty, IResolveValue>();

        public SourceContext(Type sourceType)
        {
            SourceType = sourceType;
        }

        public Type SourceType { get; private set; }

        public bool WillResolve(IDescribeMappableProperty mappableProperty)
        {
            var prop = prop2Resolver.Keys.FirstOrDefault(its => its.Name == mappableProperty.Name);
            if (prop == null)
                return false;

            return prop.PropertyType.IsCustomType() == false ||
                   IsCustomTypeAndResolverIsNotBasedOnPropertyName(prop);
//            /*for now, we are just answering this question by the property's name*/
//            return prop2Resolver.Keys.Select(its => its.Name).Contains(mappableProperty.Name);
        }
        private bool IsCustomTypeAndResolverIsNotBasedOnPropertyName(IDescribeMappableProperty prop)
        {
            var resolver = prop2Resolver[prop];
            return prop.PropertyType.IsCustomType() &&
                   typeof(RequiresComponentMappingResolver).IsInstanceOfType(resolver)==false &&
                   typeof (IOverrideable).IsInstanceOfType(resolver) == false &&
                   typeof (SourcedPropertyNameResolver).IsInstanceOfType(resolver) == false;
        }

        public IResolveValue GetResolver(IDescribeMappableProperty mappableProperty)
        {
            /*for now, we are just answering this question by the property's name*/
            var key = prop2Resolver.Keys.First(its => string.Equals(its.Name,mappableProperty.Name));
            return prop2Resolver[key];
        }

        public void SetDestinationContext(IDescribeMappableProperty[] destinationProperties)
        {
            var sourcePropertyNames = SourceType.GetProperties().Select(its => its.Name);
            foreach (var destinationProperty in destinationProperties.Where(its => sourcePropertyNames.Contains(its.Name)))
            {
                prop2Resolver[destinationProperty] = 
                    destinationProperty.PropertyType.IsCustomType() ? (IResolveValue) new RequiresComponentMappingResolver():
                    new OverrideablePropertyNameResolver(destinationProperty.Name);
            }
        }
        public bool TrySetResolver(IDescribeMappableProperty prop,IResolveValue resolver)
        {
            IResolveValue current;
            if (prop2Resolver.TryGetValue(prop, out current) ==false || current is IOverrideable)
            {
                prop2Resolver[prop] = resolver;
                return true;
            }
            return false;
        }
        public void SetResolver(IDescribeMappableProperty prop, IResolveValue resolver)
        {
            AssertOverrideable(prop);
            prop2Resolver[prop] = resolver;
        }

        private void AssertOverrideable(IDescribeMappableProperty prop)
        {
            IResolveValue resolver;
            if(prop2Resolver.TryGetValue(prop,out resolver)==false || resolver is IOverrideable)
                return;
            throw new MappingConfigurationException("Destination property '{0}' already has a manually, or conventioned, configured resolver from source type '{1}'",prop,SourceType);
        }

        public void Accept(IVisitCacheable visitor)
        {
            var cachingResolver = new CachingPropertyNameResolver();
            foreach (var resolver in prop2Resolver.Values)
            {
                cachingResolver.TryCache(SourceType,visitor,resolver);
            }
        }

        public SourcedConvention ApplyConvention(Convention convention)
        {
            return convention.Apply(SourceType);
        }
 
        public IDescribeMappableProperty CreateProperty(IDescribeMappableProperty destinationProperty)
        {
            var resolver = prop2Resolver[destinationProperty];
            var redirected = resolver as IRedirected;
            var srcPropName = redirected == null ? destinationProperty.Name : redirected.SourceProperty.Name;
            var propInfo = SourceType.GetProperty(srcPropName);
            if(propInfo==null)
                throw new MappingConfigurationException("Cannot find property '{0}' for source '{1}'",destinationProperty,SourceType);
            return new MappableProperty(propInfo);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            foreach (var resolver in prop2Resolver.Values.OfType<IBindable>())
            {
                resolver.Bind(configurations);
            }
            
        }

        public MissingProperties Validate()
        {
            var missing = new MissingProperties();
            foreach (var validatable in prop2Resolver.Values.OfType<IValidatable>())
            {
                missing=missing.Merge(validatable.Validate());
            }
            
            return missing;
        }

        public void Assert()
        {
            foreach (var validatable in prop2Resolver.Values.OfType<IValidatable>())
            {
                validatable.Assert();
            }
        }
        public override string ToString()
        {
            return GetType() + " for " + SourceType;
        }

        public bool RequiresConfigurationFor(IDescribeMappableProperty destinationProperty)
        {
            return prop2Resolver.ContainsKey(destinationProperty) &&
                   prop2Resolver[destinationProperty] is RequiresComponentMappingResolver;
        }

        private class RequiresComponentMappingResolver:IResolveValue,IOverrideable
        {
            public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
            {
                throw new MappingExecutionException("{0} requires a configuration which was not provided.",destinationProperty);
            }
        }
    }

}