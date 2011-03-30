using System.Collections;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class ListResolver : IResolveValue
    {
        private readonly IDescribeMappableProperty destinationProperty;
        private readonly ICreateExecutableMapping executor;
        private readonly IActivator activator;

        private readonly IDescribeMappableProperty sourceProperty;

        public ListResolver(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty, ICreateExecutableMapping executor, IActivator activator)
        {
            this.sourceProperty = sourceProperty;
            this.destinationProperty = destinationProperty;
            this.executor = executor;
            this.activator = activator;
        }


        public Result TryResolve(IResolutionContext context)
        {
            var collectionContext = context.Nested(destinationProperty, sourceProperty);
            var src = (IList) context.GetSourcePropertyValue(sourceProperty.Name);
            var dest = (IList)activator.CreateCollectionInstance(destinationProperty.PropertyType, src.Count);
            for (int i = 0; i < src.Count; i++)
            {
                var sourceElement = sourceProperty.ElementAt(i);
                var elementContext = collectionContext.Nested(destinationProperty.ElementAt(i),sourceElement);
                var nestedExecutor = this.executor.CreateExecutableMapping(sourceElement.PropertyType);
                nestedExecutor.Execute(elementContext);
                dest.AddElement(elementContext.Destination,i);
            }

            return new Result(true, dest);
        }
        public override string ToString()
        {
            return GetType() + " for Cfg=" + executor + ", DestinationProperty="+destinationProperty + ",SourceProperty=" + sourceProperty;
        }
    }
}