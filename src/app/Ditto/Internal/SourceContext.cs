using System;
using System.Collections.Generic;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class SourceContext : IContainResolvers,ICacheable,IValidatable,IBindable
    {
        private readonly Dictionary<IDescribeMappableProperty, IResolveValue> destinationProperty2Resolver = new Dictionary<IDescribeMappableProperty, IResolveValue>();

        public SourceContext(Type sourceType)
        {
            SourceType = sourceType;
        }

        public Type SourceType { get; private set; }

        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            var prop = destinationProperty2Resolver.Keys.FirstOrDefault(its => its.Name == destinationProperty.Name);
            if (prop == null)
                return false;

            return prop.PropertyType.IsCustomType() == false ||
                   IsCustomTypeAndCustomResolverSet(prop);
        }
        private bool IsCustomTypeAndCustomResolverSet(IDescribeMappableProperty prop)
        {
            var resolver = destinationProperty2Resolver[prop];
            return prop.PropertyType.IsCustomType() &&
                   typeof (RequiresComponentMappingResolver).IsInstanceOfType(resolver) == false;
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            /*for now, we are just answering this question by the property's name*/
            var key = destinationProperty2Resolver.Keys.First(its => string.Equals(its.Name,destinationProperty.Name));
            return destinationProperty2Resolver[key];
        }

        /// <summary>
        /// Sets the destination context for this source. This selects <paramref name="destinationProperties"/> which are pertinent to this instance's
        /// <c>SourceType</c>. This selection is based upon matching property names. Use a Redirecting call in configuration to get around this.
        /// </summary>
        /// <param name="destinationProperties">The destination properties.</param>
        public void SetDestinationContext(IDescribeMappableProperty[] destinationProperties)
        {
            var sourcePropertyNames = SourceType.GetProperties().Select(its => its.Name);
            foreach (var destinationProperty in destinationProperties.Where(its => sourcePropertyNames.Contains(its.Name)))
            {
                destinationProperty2Resolver[destinationProperty] = 
                    destinationProperty.PropertyType.IsCustomType() ? (IResolveValue) new RequiresComponentMappingResolver():
                    new OverrideablePropertyNameResolver(destinationProperty.Name);
            }
        }
        public bool TrySetResolver(IDescribeMappableProperty destinationProperty,IResolveValue resolver)
        {
            IResolveValue current;
            if (destinationProperty2Resolver.TryGetValue(destinationProperty, out current) ==false || current is IOverrideable)
            {
                destinationProperty2Resolver[destinationProperty] = resolver;
                return true;
            }
            return false;
        }
        public void SetResolver(IDescribeMappableProperty destinationProperty, IResolveValue resolver)
        {
            AssertOverrideable(destinationProperty);
            destinationProperty2Resolver[destinationProperty] = resolver;
        }

        private void AssertOverrideable(IDescribeMappableProperty destinationProperty)
        {
            IResolveValue resolver;
            if(destinationProperty2Resolver.TryGetValue(destinationProperty,out resolver)==false || resolver is IOverrideable)
                return;
            throw new MappingConfigurationException("Destination property '{0}' already has a manually, or conventioned, configured resolver from source type '{1}'",destinationProperty,SourceType);
        }

        public void Accept(IVisitCacheable visitor)
        {
            var cachingResolver = new CachingPropertyNameResolver();
            foreach (var resolver in destinationProperty2Resolver.Values)
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
            var resolver = destinationProperty2Resolver[destinationProperty];
            var redirected = resolver as IRedirected;
            var srcPropName = redirected == null ? destinationProperty.Name : redirected.SourceProperty.Name;
            var sourceProperty = SourceType.GetProperty(srcPropName);
            if(sourceProperty==null)
                throw new MappingConfigurationException("Cannot find property '{0}' for source '{1}'",destinationProperty,SourceType);
            return new MappableProperty(sourceProperty);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            foreach (var resolver in destinationProperty2Resolver.Values.OfType<IBindable>())
            {
                resolver.Bind(configurations);
            }
            
        }

        public MissingProperties Validate()
        {
            var missing = new MissingProperties();
            foreach (var validatable in destinationProperty2Resolver.Values.OfType<IValidatable>())
            {
                missing=missing.Merge(validatable.Validate());
            }
            
            return missing;
        }

        public void Assert()
        {
            foreach (var validatable in destinationProperty2Resolver.Values.OfType<IValidatable>())
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
            return destinationProperty2Resolver.ContainsKey(destinationProperty) &&
                   destinationProperty2Resolver[destinationProperty] is RequiresComponentMappingResolver;
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