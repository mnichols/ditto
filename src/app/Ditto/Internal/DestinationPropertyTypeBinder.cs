using System.Collections.Generic;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class DestinationPropertyTypeBinder:IBinder
    {
        private readonly ICreateResolver resolvers;

        public DestinationPropertyTypeBinder(ICreateResolver resolvers)
        {
            this.resolvers = resolvers;
            Logger = new NullLogFactory();
        }

        public ILogFactory Logger { get; set; }

        
        public void Bind(BindableConfiguration bindableConfiguration,params ICreateExecutableMapping[] configurations)
        {
            if (configurations == null || configurations.Length == 0)
                return;

            var children = configurations.ToDictionary(k => new ConfigurationPropertyCriterion(k), v => v);
            foreach (var childCfg in children)
            {
                KeyValuePair<ConfigurationPropertyCriterion, ICreateExecutableMapping> cfg = childCfg;
                foreach (var destinationProperty in bindableConfiguration.DestinationProperties.Where(destProp => cfg.Key.IsSatisfiedBy(destProp)))
                {
                    var copy = destinationProperty;
                    foreach (var sourceContext in bindableConfiguration.SourceContexts.Where(sourceContext => sourceContext.Matches(copy,cfg.Value.DestinationType)))
                    {
                        Logger.Create(this).Debug("Extending configuration on '{0}' from '{1}' for property '{2}'", bindableConfiguration.DestinationType, childCfg.Value.DestinationType, copy.Name);
                        var resolverContext = new ResolverContext(sourceContext.CreateProperty(destinationProperty), destinationProperty, childCfg.Value);
                        if(sourceContext.TrySetResolver(destinationProperty, resolvers.CreateRedirectingConfigurationResolver(resolverContext))==false)
                        {
                            Logger.Create(this).Info("{0} will not be overriden on {1} using {2}", destinationProperty, sourceContext, resolverContext);
                        }
                    }
                }
            }
        }
    }
}