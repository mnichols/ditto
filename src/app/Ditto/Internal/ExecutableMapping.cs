using System;
using System.Linq;

namespace Ditto.Internal
{
    public class ExecutableMapping : IExecuteMapping
    {
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly IContainResolvers resolverContainer;

        public ExecutableMapping(Type destinationType, IContainResolvers resolverContainer,IDescribeMappableProperty[] destinationProperties)
        {
            DestinationType = destinationType;
            this.resolverContainer = resolverContainer;
            this.destinationProperties = destinationProperties;
        }

        public Type DestinationType { get; set; }


        public void Execute(IResolutionContext context)
        {
            foreach (var prop in destinationProperties.Where(p => resolverContainer.WillResolve(p)))
            {
                var assignment = context.BuildValueAssignment(prop);
                var resolver = resolverContainer.GetResolver(prop);
                var result = resolver.TryResolve(context, prop);
                assignment.SetValue(result);
            }
        }
    }
}