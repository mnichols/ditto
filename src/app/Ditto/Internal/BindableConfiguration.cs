using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    /// <summary>
    ///   Intermediate representation of configuration which provide the executable mapping, as well as validation and caching hooks.
    /// </summary>
    public class BindableConfiguration : ICreateExecutableMapping, IValidatable, ICacheable, IBindable
    {
        private readonly Dictionary<Type, IContainResolvers> sourceType2ResolverContainer =new Dictionary<Type, IContainResolvers>();
        private readonly Dictionary<Type,Type> source2AlternateTypes=new Dictionary<Type, Type>();

        public BindableConfiguration(DestinationConfigurationMemento snapshot)
        {
            DestinationType = snapshot.DestinationType;
            DestinationProperties = snapshot.DestinationProperties;
            SourceContexts = snapshot.SourceContexts;
            SourcedConventions =
                snapshot.Conventions.SelectMany(cnv => snapshot.SourceContexts.Select(ctx => ctx.ApplyConvention(cnv))).
                    ToArray();
            Logger = new NullLogFactory();
            InitializeResolverContainers();
        }

        public IDescribeMappableProperty[] DestinationProperties { get; private set; }
        public ILogFactory Logger { get; set; }
        public SourceContext[] SourceContexts { get; private set; }
        public SourcedConvention[] SourcedConventions { get; private set; }
        
        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            foreach (var sourceContext in SourceContexts)
            {
                sourceContext.Bind(configurations);
            }
            foreach (var sourcedConvention in SourcedConventions)
            {
                sourcedConvention.Bind(configurations);
            }
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
            visitor.Visit(this);
        }

        public Type DestinationType { get; private set; }

        public IExecuteMapping CreateExecutableMapping(Type sourceType)
        {
            var typeToUse = DemandSourceContextDeep(sourceType);
            var resolversContainer = sourceType2ResolverContainer[typeToUse];
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

        private void InitializeResolverContainers()
        {
            foreach (var resolverContainer in ResolverPrecedence.ToResolverContainer(this))
            {
                sourceType2ResolverContainer.Add(resolverContainer.Key, resolverContainer.Value);
            }
        }

        private Type DemandSourceContextDeep(Type sourceType)
        {

            if (SourceContexts.Length == 0)
                throw new DittoConfigurationException(
                    "Sources have not been setup for '{0}'. Did you forget to call 'From'?", DestinationType);

            var deepThots = new SourceContextDiscovery(sourceType);
            var context = SourceContexts.FirstOrDefault(deepThots.IsSatisfiedBy);
            if(context!=null)
            {
                source2AlternateTypes[sourceType] = deepThots.TypeToUse;
                return deepThots.TypeToUse;
            }

            throw new DittoConfigurationException("'{0}' has not been setup as a source for '{1}'.", sourceType,
                                                  DestinationType);
        }

        public override string ToString()
        {
            return GetType() + " for '" + DestinationType + "'";
        }

        public void SourceResolverContainer(Type sourceType, IContainResolvers sourcedResolverContainer)
        {
            //it might seem superflous to have the resolver containers assigned twice, but caching shouldn't be an requirement
            sourceType2ResolverContainer[sourceType] = sourcedResolverContainer;
        }
    }
}