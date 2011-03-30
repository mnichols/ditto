using System;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class ExecutableMapping : IExecuteMapping
    {
        private readonly IResolverContainer resolverContainer;
        private readonly IDescribeMappableProperty[] destinationProperties;
        public ExecutableMapping(Type destinationType, IResolverContainer resolverContainer, IDescribeMappableProperty[] destinationProperties)
        {
            DestinationType = destinationType;
            this.resolverContainer = resolverContainer;
            this.destinationProperties = destinationProperties;
        }

        public Type DestinationType { get; set; }


        public void Execute(IResolutionContext context)
        {
            foreach (var prop in destinationProperties.Where(p=>resolverContainer.WillResolve(p)))
            {
                var assignment = context.Scope(prop);
                var resolver = resolverContainer.GetResolver(prop);
                Result result;
                var dependOnDestinationPropertyState = resolver as IDependOnDestinationPropertyState;

                if(dependOnDestinationPropertyState==null)
                {
                    result = resolver.TryResolve(context);
                }
                else
                {
                    result = dependOnDestinationPropertyState.ResolveBasedOn(context, prop);
                }
                
                assignment.SetValue(result);
            }
        }
    }
}