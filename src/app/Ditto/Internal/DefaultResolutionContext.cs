using System;

namespace Ditto.Internal
{
    public class DefaultResolutionContext : IResolutionContext
    {
        private readonly IActivator activator;
        private readonly IContextualizeResolution contextualizer;
        private readonly object destination;
        private readonly object source;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultResolutionContext(object source, 
            object destination, 
            IActivator activator,
            IContextualizeResolution contextualizer,
            ICreateValueAssignment valueAssignments,
            IInvoke invoke)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (activator == null)
                throw new ArgumentNullException("activator");
            if (valueAssignments == null)
                throw new ArgumentNullException("valueAssignments");
            this.source = source;
            this.destination = destination;
            this.activator = activator;
            this.contextualizer = contextualizer;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext Nested(IDescribeMappableProperty destinationProperty)
        {
            var dest = invoke.GetValue(destinationProperty.Name, Destination) ?? activator.CreateInstance(destinationProperty.PropertyType);
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

        public IAssignValue Scope(IDescribeMappableProperty destinationProperty)
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
                    return activator.CreateInstance(destinationProperty.PropertyType);
                }
                return invoke.GetValue(destinationProperty.Name, Destination) ?? activator.CreateInstance(destinationProperty.PropertyType);
            }
            return activator.CreateCollectionInstance(destinationProperty.PropertyType, collectionSpec.GetLength(src));
        }
        
    }
}