using System;

namespace Ditto.Internal
{
    public class DefaultResolutionContext : IResolutionContext
    {
        private readonly IActivate activate;
        private readonly IContextualizeResolution contextualizer;
        private readonly object destination;
        private readonly object source;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultResolutionContext(object source, 
            object destination, 
            IActivate activate,
            IContextualizeResolution contextualizer,
            ICreateValueAssignment valueAssignments,
            IInvoke invoke)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (activate == null)
                throw new ArgumentNullException("activate");
            if (valueAssignments == null)
                throw new ArgumentNullException("valueAssignments");
            this.source = source;
            this.destination = destination;
            this.activate = activate;
            this.contextualizer = contextualizer;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext Nested(IDescribeMappableProperty destinationProperty)
        {
            var dest = invoke.GetValue(destinationProperty.Name, Destination) ?? activate.CreateInstance(destinationProperty.PropertyType);
            return contextualizer.CreateContext(Source, dest);
        }

        public Type SourceType
        {
            get { return source.GetType(); }
        }

        public object Source
        {
            get { return source; }
        }

        public object Destination
        {
            get { return destination; }
        }

        public IAssignValue BuildValueAssignment(IDescribeMappableProperty destinationProperty)
        {
            if (destinationProperty == null)
                throw new ArgumentNullException("destinationProperty");
            return valueAssignments.Create(destination, destinationProperty);
        }

        public object GetSourcePropertyValue(string propertyName)
        {
            return invoke.GetValue(propertyName, Source);
        }
        public object GetDestinationPropertyValue(string propertyName)
        {
            return invoke.GetValue(propertyName, destination);
        }

        public IResolutionContext Nested(IDescribeMappableProperty destinationProperty, IDescribeMappableProperty sourceProperty)
        {
            var src = GetSourceValue(Source, sourceProperty);
            var dest = GetDestValue(src, destinationProperty);
            return contextualizer.CreateContext(src, dest);
        }
        private object GetSourceValue(object src,IDescribeMappableProperty sourceProperty)
        {
            var collectionSpec = new SupportedCollectionTypeSpecification();

            if (collectionSpec.IsSatisfiedBy(src) == false)
            {
                return invoke.GetValue(sourceProperty.Name, Source);
            }
            return collectionSpec.GetValue(src, sourceProperty as IDescribePropertyElement);
        }
        private object GetDestValue(object src,IDescribeMappableProperty destinationProperty)
        {
            var collectionSpec = new SupportedCollectionTypeSpecification();

            if(collectionSpec.IsSatisfiedBy(src)==false)
            {
                if(collectionSpec.IsElement(destinationProperty))
                {
                    return activate.CreateInstance(destinationProperty.PropertyType);
                }
                return invoke.GetValue(destinationProperty.Name, Destination) ?? activate.CreateInstance(destinationProperty.PropertyType);
            }
            return activate.CreateCollectionInstance(destinationProperty.PropertyType, collectionSpec.GetLength(src));
        }
        
    }
}