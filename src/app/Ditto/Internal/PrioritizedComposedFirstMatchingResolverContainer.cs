using System;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class PrioritizedComposedFirstMatchingResolverContainer:IResolverContainer
    {
        private IResolverContainer[] containers;

        public PrioritizedComposedFirstMatchingResolverContainer(IResolverContainer[] containers)
        {
            if(containers==null || containers.Length==0)
            {
                this.containers=new IResolverContainer[]{new NullResolverContainer()};
            }
            else
            {
                this.containers = containers;    
            }
            
        }

        public bool WillResolve(IDescribeMappableProperty mappableProperty)
        {
            return containers.Any(it => it.WillResolve(mappableProperty));
        }

        public IResolveValue GetResolver(IDescribeMappableProperty mappableProperty)
        {
            /*
             * Like its name implies, this container will match the first resolver that 'WillResolve'. 
             * If the resolver is IOverrideable it will try to continue to the next resolver that WillResolve, returning that candidate if none else is found.
             * This means the ordering is imporant in the construction of this resolver.
             */
            IResolveValue candidate=null;
            foreach (var container in containers)
            {
                if (container.WillResolve(mappableProperty)==false)
                    continue;
                candidate= container.GetResolver(mappableProperty);
                if(candidate is IOverrideable) /*system resolver...keep trying but remember it in case none else be found*/
                    continue;
                return candidate;
            }
            if (candidate != null)
                return candidate;
            throw new InvalidOperationException("This container does not having any matching resolvers for '" + mappableProperty + "'");
        }
    }
}