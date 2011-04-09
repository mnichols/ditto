using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class BindingDestinationConfigurationContainer:IBindConfigurations,ICreateMappingCommand,IValidatable,ICacheable
    {
        private readonly IProvideBinders binders;
        private readonly IMapCommandFactory mapCommands;
        private readonly ICollection<BindableConfiguration> configurations;
        private Dictionary<Type,BindableConfiguration> bindableConfigurations=new Dictionary<Type, BindableConfiguration>();
        public BindingDestinationConfigurationContainer(IProvideBinders binders, IMapCommandFactory mapCommands, IProvideBindableConfigurations bindableConfigurations)
        {
            this.binders = binders;
            this.mapCommands = mapCommands;
            this.configurations = bindableConfigurations.GetBindableConfigurations();
            Logger = new NullLogFactory();
        }
        public ILogFactory Logger { get; set; }
        public void Bind()
        {
            var allBinders = binders.Create();
            bindableConfigurations = configurations.ToDictionary(k => k.DestinationType,v => v);
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

            var executionCreator = cfg as ICreateExecutableMapping;
            if (executionCreator == null)
            {
                //TODO:brittle
                throw new InvalidOperationException("Expected the configuration to create an executable mapping...but it can't.");
            }
            return mapCommands.Create(executionCreator.CreateExecutableMapping(sourceType));
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
            foreach (var config in configurations)
            {
                var cacheable = config as ICacheable;
                if (cacheable == null)
                    continue;
                cacheable.Accept(visitor);
            }
        }
    }
}