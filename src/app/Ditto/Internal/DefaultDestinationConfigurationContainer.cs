using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class DefaultDestinationConfigurationContainer : IContainDestinationConfiguration, IValidatable, ICreateMappingCommand, ICacheable,IBindConfigurations
    {
        private readonly Dictionary<Type, IValidatable> registeredConfigurations = new Dictionary<Type, IValidatable>();
        private readonly IMapCommandFactory mapCommandFactory;
        private readonly IProvideConventions conventions;
        private readonly ICreateDestinationConfiguration configurations;
        public ILogFactory Logger { get; set; }
        public DefaultDestinationConfigurationContainer(IMapCommandFactory mapCommandFactory, IProvideConventions conventions, ICreateDestinationConfiguration configurations)
        {
            this.mapCommandFactory = mapCommandFactory;
            this.conventions = conventions;
            this.configurations = configurations;
            Logger = new NullLogFactory();
        }
        private void TryApplyGlobalConventions(IApplyConventions configuration)
        {
            if(configuration==null || conventions==null)
                return;
            conventions.Inject(configuration);
        }
        public IConfigureDestination Map(Type destinationType)
        {
            IValidatable cfg;
            if (registeredConfigurations.TryGetValue(destinationType, out cfg) == false)
            {
                registeredConfigurations.Add(destinationType, cfg = (IValidatable)configurations.Create(destinationType));
                TryApplyGlobalConventions(cfg as IApplyConventions);    
            }
            return (DestinationConfiguration)cfg;
        }

        public IConfigureDestination<TDest> Map<TDest>()
        {
            IValidatable cfg;
            if (registeredConfigurations.TryGetValue(typeof(TDest), out cfg) == false)
            {
                registeredConfigurations.Add(typeof(TDest), cfg = (IValidatable)configurations.Create<TDest>());
                TryApplyGlobalConventions(cfg as IApplyConventions);
            }
            return (DestinationConfiguration<TDest>)cfg;
        }

        public bool HasConfigurationFor(Type destinationType)
        {
            return registeredConfigurations.ContainsKey(destinationType);
        }

        public ICollection<ICreateBindableConfiguration> GetBindableConfigurationCreators()
        {
            return registeredConfigurations.Values.OfType<ICreateBindableConfiguration>().ToArray();
        }

        public MissingProperties Validate()
        {
            var missing = new MissingProperties();
            foreach (var validatable in registeredConfigurations)
            {
                var theseMissing = validatable.Value.Validate();
                missing=missing.Merge(theseMissing);
            }
            
            
            Logger.Create(GetType()).Info("There are '{0}' missing properties after validation.{1}{2}", missing.Count(),Environment.NewLine, missing.GetFormattedMissingProperties());
            return missing;
        }
        public void Assert()
        {
            var missing = Validate();
            missing.TryThrow();
        }

        public IMapCommand CreateCommand(Type destinationType, Type sourceType)
        {
            IValidatable cfg;
            if(registeredConfigurations.TryGetValue(destinationType,out cfg)==false)
            {
                throw new MappingExecutionException("Could not find mapping configuration for '{0}'. Please check that a configuration exists for this type and that the mapping engine is initialized.",destinationType);
            }

            var executionCreator = cfg as ICreateExecutableMapping;
            if (executionCreator == null)
            {
                //TODO:brittle
                throw new InvalidOperationException("Expected the configuration to create an executable mapping...but it can't.");
            }
            return mapCommandFactory.Create(executionCreator.CreateExecutableMapping(sourceType));
        }

        public void Accept(IVisitCacheable visitor)
        {
            foreach (var config in registeredConfigurations)
            {
                var cacheable = config.Value as ICacheable;
                if (cacheable == null)
                    continue;
                cacheable.Accept(visitor);
            }
        }

        public void Bind()
        {
            
            var bindableConfigurations = registeredConfigurations.Values.OfType<IBindable>().ToArray();
            var flattened = registeredConfigurations.Values.OfType<ICreateExecutableMapping>().ToArray();
            Logger.Create(this).Debug("Binding {0} destination configurations to {1} extending configurations",bindableConfigurations.Length,flattened.Length);
            foreach (var item in bindableConfigurations)
            {
                item.Bind(flattened);
            }
        }

        public BindingDestinationConfigurationContainer ToBinding(IProvideBinders binders,IMapCommandFactory mapCommands)
        {
            return new BindingDestinationConfigurationContainer(binders, mapCommands,this){Logger = Logger};
        }
    }
}