using System.Collections.Generic;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class ListPropertyBinder:IBinder
    {
        private readonly ICreateResolver resolvers;
        public ListPropertyBinder(ICreateResolver resolvers)
        {
            this.resolvers = resolvers;
            Logger = new NullLogFactory();
        }

        public ILogFactory Logger { get; set; }
        
        
        public IEnumerable<IDescribeMappableProperty> GetCandidateDestinationProperties(BindableConfiguration bindableConfiguration)
        {
            return bindableConfiguration.DestinationProperties
                .Where(property => new ListPropertyCriterion().IsSatisfiedBy(property)).ToArray();
        }

        public void Bind(BindableConfiguration bindableConfiguration, params ICreateExecutableMapping[] configurations)
        {
            var candidates = GetCandidateDestinationProperties(bindableConfiguration);
            var candidateElements = candidates.Select(its => its.PropertyType.ElementType());
            var filteredConfigs = configurations.Where(cfg => candidateElements.Contains(cfg.DestinationType));
            var candidate2Cfg=new Dictionary<IDescribeMappableProperty, List<ICreateExecutableMapping>>();
            foreach (var cand in candidates)
            {
                List<ICreateExecutableMapping> cfgs;
                if(candidate2Cfg.TryGetValue(cand,out cfgs)==false)
                {
                    candidate2Cfg.Add(cand,cfgs=new List<ICreateExecutableMapping>());
                }
                cfgs.AddRange(filteredConfigs);
            }
            foreach (var candidate in candidate2Cfg)
            {
                var copy = candidate;
                foreach (var sourceContext in bindableConfiguration.SourceContexts.Where(it => it.WillResolve(copy.Key)))
                {
                    /*be sure we set resolver for each property that has a list element matching*/
                    foreach (var createExecutableMapping in candidate.Value)
                    {
                        var resolverContext = new ResolverContext(sourceContext.CreateProperty(copy.Key), copy.Key, createExecutableMapping);
                        var resolver = resolvers.CreateListResolver(resolverContext);
                        if(sourceContext.TrySetResolver(candidate.Key, resolver)==false)
                        {
                            Logger.Create(this).Info("{0} will not be overriden on {1} using {2}",candidate.Key,sourceContext,resolver);
                        }    
                    }
                    
                }
            }
        }
    }
}