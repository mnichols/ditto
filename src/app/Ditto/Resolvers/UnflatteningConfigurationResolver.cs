using System;
using Ditto.Internal;
using System.Linq;

namespace Ditto.Resolvers
{
    public class UnflatteningConfigurationResolver : IResolveValue,IBindable
    {
        private ICreateExecutableMapping executor;
        private Type destinationType;

        internal UnflatteningConfigurationResolver(Type destinationType)
        {
            this.destinationType = destinationType;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if (executor == null)
            {
                throw new DittoConfigurationException("UnflatteningResolver never got an executable mapping on destination type '{0}'",destinationType);
            }
            //we need to keep the SOURCE the same, but change the destination
            var nested = context.Nested(destinationProperty);
            var executable = executor.CreateExecutableMapping(nested.SourceType);
            executable.Execute(nested);
            return new Result(true, nested.Destination);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            if(executor==null)
            {
                executor = configurations.FirstOrDefault(its => its.DestinationType == destinationType);
            }
        }
    }
}