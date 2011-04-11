using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{

    public class BindingDestinationConfigurationContainer:IBindConfigurations,ICreateMappingCommand,IValidatable,ICacheable
    {

        private readonly IProvideBinders binders;
        private readonly IMapCommandFactory mapCommands;
        private readonly ICreateBindableConfiguration bindableConfigurationsFactory;
        private Dictionary<Type,BindableConfiguration> bindableConfigurations=new Dictionary<Type, BindableConfiguration>();
        private ICollection<DestinationConfigurationMemento> snapshots;

        public BindingDestinationConfigurationContainer(IProvideBinders binders, 
            IMapCommandFactory mapCommands, 
            IProvideDestinationConfigurationSnapshots destinationConfigurationSnapshots,
            ICreateBindableConfiguration bindableConfigurationsFactory)
        {
            this.binders = binders;
            this.mapCommands = mapCommands;
            this.snapshots = destinationConfigurationSnapshots.TakeSnapshots();
            this.bindableConfigurationsFactory = bindableConfigurationsFactory;
            Logger = new NullLogFactory();
        }
        public ILogFactory Logger { get; set; }
        public void Bind()
        {
            bindableConfigurations = snapshots.ToDictionary(mem => mem.DestinationType,mem =>bindableConfigurationsFactory.CreateBindableConfiguration(mem));
            var allBinders = binders.Create();
            var executable = bindableConfigurations.Values.OfType<ICreateExecutableMapping>().ToArray();
            Logger.Create(this).Debug("Binding {0} destination configurations to {1} extending configurations", bindableConfigurations.Count, executable.Length);
            foreach (var binder in allBinders)
            {
                foreach (var bindableConfiguration in bindableConfigurations)
                {
                    binder.Bind(bindableConfiguration.Value,executable);
                }
            }
            
        }

        public IMapCommand CreateCommand(Type sourceType, Type destinationType)
        {
            BindableConfiguration cfg;
            if (bindableConfigurations.TryGetValue(destinationType, out cfg) == false)
            {
                throw new MappingExecutionException("Could not find mapping configuration for '{0}'. Please check that a configuration exists for this type and that the mapping engine is initialized.", destinationType);
            }
            return mapCommands.Create(cfg.CreateExecutableMapping(sourceType));
        }

        public MissingProperties Validate()
        {
            var missing = new MissingProperties();
            foreach (var validatable in bindableConfigurations.Values.OfType<IValidatable>())
            {
                var theseMissing = validatable.Validate();
                missing = missing.Merge(theseMissing);
            }


            Logger.Create(GetType()).Info("There are '{0}' missing properties after validation.{1}{2}", missing.Count(), Environment.NewLine, missing.GetFormattedMissingProperties());
            return missing;
        }
        public void Assert()
        {
            var missing = Validate();
            missing.TryThrow();
        }

        public void Accept(IVisitCacheable visitor)
        {
            foreach (var config in bindableConfigurations.Values)
            {
                var cacheable = config as ICacheable;
                if (cacheable == null)
                    continue;
                cacheable.Accept(visitor);
            }
        }
    }
}