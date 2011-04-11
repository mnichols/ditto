using System;
using System.Collections.Generic;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class PrioritizedComposedFirstMatchingResolverContainer:IContainResolvers
    {
        private IContainResolvers[] containers;

        private Dictionary<IDescribeMappableProperty, IResolveValue> cachedResolvers =new Dictionary<IDescribeMappableProperty, IResolveValue>();
        public PrioritizedComposedFirstMatchingResolverContainer(IContainResolvers[] containers)
        {
            if(containers==null || containers.Length==0)
            {
                this.containers=new IContainResolvers[]{new NullResolverContainer()};
            }
            else
            {
                this.containers = containers;    
            }
            
        }

        public bool WillResolve(IDescribeMappableProperty destinationProperty)
        {
            return cachedResolvers.ContainsKey(destinationProperty) ||
                containers.Any(it => it.WillResolve(destinationProperty));
        }

        public IResolveValue GetResolver(IDescribeMappableProperty destinationProperty)
        {
            
            /*
             * Like its name implies, this container will match the first resolver that 'WillResolve'. 
             * If the resolver is IOverrideable it will try to continue to the next resolver that WillResolve, returning that candidate if none else is found.
             * This means the ordering is imporant in the construction of this resolver.
             */
            IResolveValue candidate;
            if (cachedResolvers.TryGetValue(destinationProperty, out candidate))
                return candidate;

            candidate = TryGetCandidate(destinationProperty);

            if (candidate != null)
            {
                cachedResolvers.Add(destinationProperty,candidate);
                return candidate;
            }
            throw new InvalidOperationException("This container does not having any matching resolvers for '" + destinationProperty + "'");
        }
        private IResolveValue TryGetCandidate(IDescribeMappableProperty mappableProperty)
        {
            IResolveValue candidate=null;
            foreach (var container in containers)
            {
                if (container.WillResolve(mappableProperty) == false)
                    continue;
                candidate = container.GetResolver(mappableProperty);
                if (typeof(IOverrideable).IsInstanceOfType(candidate) == false) 
                    return candidate;
                /*default resolver...keep trying but remember it in case none else be found*/
            }
            return candidate;
        }
        public void Source(IDescribeMappableProperty[] destinationProperties)
        {
            var cacheable = destinationProperties.Where(WillResolve).ToArray();
            foreach (var property in cacheable)
            {
                cachedResolvers[property]=TryGetCandidate(property);
            }
            
        }
    }
}