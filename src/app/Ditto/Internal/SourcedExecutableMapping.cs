using System;

namespace Ditto.Internal
{
    public class SourcedExecutableMapping:IExecuteMapping
    {
        public Type SourceType { get; private set; }
        public Type DestinationType { get; private set; }

        public SourcedExecutableMapping(Type sourceType,Type destinationType, IContainResolvers resolverContainer, IDescribeMappableProperty[] destinationProperties)
        {
            
        }
        public void Execute(IResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}