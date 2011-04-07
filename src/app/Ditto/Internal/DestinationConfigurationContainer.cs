using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class DestinationConfigurationContainer : IContainDestinationConfiguration
    {
        private readonly Dictionary<Type, ICreateBindableConfiguration> registeredConfigurations = new Dictionary<Type, ICreateBindableConfiguration>();
        private readonly IProvideConventions conventions;
        private readonly ICreateDestinationConfiguration configurations;
        public ILogFactory Logger { get; set; }
        public DestinationConfigurationContainer(IProvideConventions conventions, ICreateDestinationConfiguration configurations)
        {
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
            ICreateBindableConfiguration cfg;
            if (registeredConfigurations.TryGetValue(destinationType, out cfg) == false)
            {
                registeredConfigurations.Add(destinationType, cfg = configurations.Create(destinationType));
                TryApplyGlobalConventions(cfg as IApplyConventions);    
            }
            return (DestinationConfiguration)cfg;
        }

        public IConfigureDestination<TDest> Map<TDest>()
        {
            ICreateBindableConfiguration cfg;
            if (registeredConfigurations.TryGetValue(typeof(TDest), out cfg) == false)
            {
                registeredConfigurations.Add(typeof(TDest), cfg = configurations.Create<TDest>());
                TryApplyGlobalConventions(cfg as IApplyConventions);
            }
            return (DestinationConfiguration<TDest>)cfg;
        }

        public bool HasConfigurationFor(Type destinationType)
        {
            return registeredConfigurations.ContainsKey(destinationType);
        }

        public ICollection<BindableConfiguration> CreateBindableConfigurations()
        {
            return registeredConfigurations.Values.Select(into=>into.CreateBindableConfiguration()).ToArray();
        }


    }
}