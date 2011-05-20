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
                throw new DittoConfigurationException("UnflatteningResolver never bound an executable mapping on component type '{0}' for destination '{1}'. " +
                    Environment.NewLine+
                    "Be sure the type '{0}' is configured as a destination with source '{2}'",destinationType,context.Destination,context.SourceType);
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