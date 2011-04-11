using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class BindableConfiguration : ICreateExecutableMapping, IValidatable,ICacheable
    {
        public BindableConfiguration(DestinationConfigurationMemento snapshot)
        {
            DestinationType = snapshot.DestinationType;
            DestinationProperties = snapshot.DestinationProperties;
            SourceContexts = snapshot.SourceContexts;
            SourcedConventions = snapshot.Conventions.SelectMany(cnv => snapshot.SourceContexts.Select(ctx => ctx.ApplyConvention(cnv))).ToArray();
            Logger = new NullLogFactory();
            SourceResolverContainers();
        }
        private Dictionary<Type,IContainResolvers> sourceType2ResolverContainer=new Dictionary<Type, IContainResolvers>();
        private void SourceResolverContainers()
        {
            foreach (var sourceContext in SourceContexts)
            {
                var copy = sourceContext;
                var sourcedConventions = new PrioritizedComposedFirstMatchingResolverContainer(
                        SourcedConventions.Where(its => its.SourceType == copy.SourceType).ToArray());
                
                var composed = new PrioritizedComposedFirstMatchingResolverContainer(new IContainResolvers[] { sourceContext, sourcedConventions });
                sourceType2ResolverContainer.Add(sourceContext.SourceType,composed);
            }
        }

        public IDescribeMappableProperty[] DestinationProperties { get; private set; }
        public SourceContext[] SourceContexts { get; private set; }
        public SourcedConvention[] SourcedConventions { get; private set; }
        public ILogFactory Logger { get; set; }
        public Type DestinationType { get; private set; }

        public IExecuteMapping CreateExecutableMapping(Type sourceType)
        {
            AssertSourceContext(sourceType);
            var resolversContainer = sourceType2ResolverContainer[sourceType];
            return new ExecutableMapping(DestinationType, resolversContainer, DestinationProperties);
        }

        public MissingProperties Validate()
        {
            var validator = new ConfigurationValidator(DestinationType,
                                                       DestinationProperties,
                                                       SourceContexts.Concat<IContainResolvers>(SourcedConventions).
                                                           ToArray());
            return validator.Validate();
        }

        public void Assert()
        {
            var missing = Validate();
            missing.TryThrow();
        }

        private void AssertSourceContext(Type sourceType)
        {
            if (SourceContexts.Length == 0)
                throw new MappingConfigurationException(
                    "Sources have not been setup for '{0}'. Did you forget to call 'From'?", DestinationType);

            if( SourceContexts.Any(its => its.SourceType == sourceType))
                return;

            throw new MappingConfigurationException("'{0}' has not been setup as a source for '{1}'.", sourceType,DestinationType);
        }
        public override string ToString()
        {
            return GetType() + " for '" + DestinationType+"'";
        }

        public void Accept(IVisitCacheable visitor)
        {
            foreach (var sourceContext in SourceContexts)
            {
                sourceContext.Accept(visitor);
            }
            foreach (var cachedDestinationProp in DestinationProperties.OfType<ICacheable>())
            {
                cachedDestinationProp.Accept(visitor);
            }
        }
    }
}