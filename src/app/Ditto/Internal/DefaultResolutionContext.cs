using System;

namespace Ditto.Internal
{
    public class DefaultResolutionContext : IResolutionContext
    {
        private readonly IContextualizeResolution contextualizer;
        private readonly object destination;
        private readonly object source;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultResolutionContext(object source, 
            object destination,
            IContextualizeResolution contextualizer,
            ICreateValueAssignment valueAssignments,
            IInvoke invoke)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (valueAssignments == null)
                throw new ArgumentNullException("valueAssignments");
            this.source = source;
            this.destination = destination;
            this.contextualizer = contextualizer;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext Nested(IDescribeMappableProperty destinationProperty)
        {
            return contextualizer.CreateNestedContext(destinationProperty, this);
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

        public IResolutionContext Nested(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty)
        {
            return contextualizer.CreateNestedParallelContext(sourceProperty, destinationProperty,this);
        }
        
    }
}