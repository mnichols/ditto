using System;
using System.Linq;

namespace Ditto.Internal
{
    public class BindableConfiguration : ICreateExecutableMapping, IBindable, IValidatable,ICacheable
    {
        public BindableConfiguration(Type destinationType, IDescribeMappableProperty[] destinationProperties,
                                     SourceContext[] sourceContext, Convention[] conventions)
        {
            DestinationType = destinationType;
            DestinationProperties = destinationProperties;
            SourceContexts = sourceContext;
            SourcedConventions = conventions.SelectMany(cnv => sourceContext.Select(ctx => ctx.ApplyConvention(cnv))).ToArray();
            Logger = new NullLogFactory();
        }

        public IDescribeMappableProperty[] DestinationProperties { get; private set; }
        public SourceContext[] SourceContexts { get; private set; }
        public SourcedConvention[] SourcedConventions { get; private set; }
        public ILogFactory Logger { get; set; }
        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var binders = new BinderFactory(new Fasterflection()){Logger = Logger};
            foreach (var binder in binders.Create())
            {
                binder.Bind(this, configurations);
            }
        }

        public Type DestinationType { get; private set; }

        public IExecuteMapping CreateExecutableMapping(Type sourceType)
        {
            var sourceContext = DemandSourceContext(sourceType);
            var sourcedConventions =
                new PrioritizedComposedFirstMatchingResolverContainer(
                    SourcedConventions.Where(its => its.SourceType == sourceType).ToArray());

            var composed =
                new PrioritizedComposedFirstMatchingResolverContainer(new IContainResolvers[] { sourceContext, sourcedConventions });
            return new ExecutableMapping(DestinationType, composed, DestinationProperties);
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

        private SourceContext DemandSourceContext(Type sourceType)
        {
            if (SourceContexts.Length == 0)
                throw new MappingConfigurationException(
                    "Sources have not been setup for '{0}'. Did you forget to call 'From'?", DestinationType);
            var ctx = SourceContexts.FirstOrDefault(its => its.SourceType == sourceType);
            if (ctx != null)
                return ctx;
            throw new MappingConfigurationException("'{0}' has not been setup as a source for '{1}'.", sourceType,
                                                    DestinationType);
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