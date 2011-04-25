using System.Collections;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class ListResolver : IResolveValue
    {
        private readonly ICreateExecutableMapping executor;
        private readonly IActivate activate;

        private readonly IDescribeMappableProperty sourceProperty;

        public ListResolver(IDescribeMappableProperty sourceProperty, ICreateExecutableMapping executor, IActivate activate)
        {
            this.sourceProperty = sourceProperty;
            this.executor = executor;
            this.activate = activate;
        }


        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            var collectionContext = context.Nested(sourceProperty, destinationProperty);
            var src = (IList) context.GetSourcePropertyValue(sourceProperty.Name);
            var dest = (IList)activate.CreateCollectionInstance(destinationProperty.PropertyType, src.Count);
            for (int i = 0; i < src.Count; i++)
            {
                var sourceElement = sourceProperty.ElementAt(i);
                var elementContext = collectionContext.Nested(sourceElement, destinationProperty.ElementAt(i));
                var nestedExecutor = this.executor.CreateExecutableMapping(sourceElement.PropertyType);
                nestedExecutor.Execute(elementContext);
                dest.AddElement(elementContext.Destination,i);
            }

            return new Result(true, dest);
        }
        
    }
}