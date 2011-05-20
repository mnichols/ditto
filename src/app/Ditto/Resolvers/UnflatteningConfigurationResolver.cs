using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class UnflatteningConfigurationResolver : IResolveValue
    {
        private readonly IProvideExecutableMappingCreator executableMappings;
        private ICreateExecutableMapping executor;

        internal UnflatteningConfigurationResolver(IProvideExecutableMappingCreator executableMappings)
        {
            this.executableMappings = executableMappings;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if (executor == null)
            {
                executor = executableMappings.GetExecutableMapping(destinationProperty.PropertyType);
            }
            //we need to keep the SOURCE the same, but change the destination
            var nested = context.Nested(destinationProperty);
            var executable = executor.CreateExecutableMapping(nested.SourceType);
            executable.Execute(nested);
            return new Result(true, nested.Destination);
        }
    }
}