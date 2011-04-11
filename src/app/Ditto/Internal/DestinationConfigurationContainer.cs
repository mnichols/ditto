using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class DestinationConfigurationContainer : IContainDestinationConfiguration,IProvideDestinationConfigurationSnapshots
    {
        private readonly Dictionary<Type, ITakeDestinationConfigurationSnapshot> registeredConfigurations = new Dictionary<Type, ITakeDestinationConfigurationSnapshot>();
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
            ITakeDestinationConfigurationSnapshot cfg;
            if (registeredConfigurations.TryGetValue(destinationType, out cfg) == false)
            {
                registeredConfigurations.Add(destinationType, cfg = configurations.Create(destinationType));
                TryApplyGlobalConventions(cfg as IApplyConventions);    
            }
            return (DestinationConfiguration)cfg;
        }

        public IConfigureDestination<TDest> Map<TDest>()
        {
            ITakeDestinationConfigurationSnapshot cfg;
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

        public ICollection<DestinationConfigurationMemento> TakeSnapshots()
        {
            return registeredConfigurations.Values.Select(into=>into.TakeSnapshot()).ToArray();
        }


    }
}