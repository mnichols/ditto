using System;
using System.Linq;
using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class ExecutableMapping : IExecuteMapping
    {
        private readonly IContainResolvers resolverContainer;
        private readonly IDescribeMappableProperty[] destinationProperties;
        public ExecutableMapping(Type destinationType, IContainResolvers resolverContainer, IDescribeMappableProperty[] destinationProperties)
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
                var result= resolver.TryResolve(context, prop);
                assignment.SetValue(result);
            }
        }
    }
}