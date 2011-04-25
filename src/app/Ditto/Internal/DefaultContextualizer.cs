using System;

namespace Ditto.Internal
{
    public class DefaultContextualizer:IContextualizeResolution
    {
        private readonly IActivate activate;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultContextualizer(IActivate activate, ICreateValueAssignment valueAssignments, IInvoke invoke)
        {
            this.activate = activate;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext CreateContext(object source, object destination)
        {
            return new DefaultResolutionContext(source, destination, this, valueAssignments,invoke);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            var destination = activate.CreateInstance(destinationType);
            return new DefaultResolutionContext(source, destination, this, valueAssignments, invoke);
        }

        public IResolutionContext CreateNestedParallelContext(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty, IResolutionContext parentContext)
        {
            var src = GetSourceValue(parentContext.Source, sourceProperty);
            var dest = GetDestValue(src,parentContext.Destination, destinationProperty);
            if (src == null && destinationProperty.IsCustomType)
                return new NullSourceContext(sourceProperty, valueAssignments, invoke, activate);
            return CreateContext(src, dest);
        }

        public IResolutionContext CreateNestedContext(IDescribeMappableProperty destinationProperty, IResolutionContext parentContext)
        {
            var dest = invoke.GetValue(destinationProperty.Name, parentContext.Destination) ?? activate.CreateInstance(destinationProperty.PropertyType);
            return CreateContext(parentContext.Source, dest);
        }

        private object GetSourceValue(object src, IDescribeMappableProperty sourceProperty)
        {
            var collectionSpec = new SupportedCollectionTypeSpecification();

            if (collectionSpec.IsSatisfiedBy(src) == false)
            {
                return invoke.GetValue(sourceProperty.Name, src);
            }
            return collectionSpec.GetValue(src, sourceProperty as IDescribePropertyElement);
        }
        private object GetDestValue(object src, object dest,IDescribeMappableProperty destinationProperty)
        {
            var collectionSpec = new SupportedCollectionTypeSpecification();

            if (collectionSpec.IsSatisfiedBy(src) == false)
            {
                if (collectionSpec.IsElement(destinationProperty))
                {
                    return activate.CreateInstance(destinationProperty.PropertyType);
                }
                return invoke.GetValue(destinationProperty.Name, dest) ?? activate.CreateInstance(destinationProperty.PropertyType);
            }
            return activate.CreateCollectionInstance(destinationProperty.PropertyType, collectionSpec.GetLength(src));
        }
    }
}