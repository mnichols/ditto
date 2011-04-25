using System;

namespace Ditto.Internal
{
    public class NullSourceContext:IResolutionContext
    {
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;
        private readonly IActivate activate;

        public NullSourceContext(IDescribeMappableProperty sourceProperty, ICreateValueAssignment valueAssignments, IInvoke invoke, IActivate activate)
        {
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
            this.activate = activate;
            SourceType = sourceProperty.PropertyType;
            Source = null;
        }
        public Type SourceType { get; private set; }
        public object Source { get; private set; }
        public object Destination { get; private set; }
        public IAssignValue BuildValueAssignment(IDescribeMappableProperty destinationProperty)
        {
            return new NullValueAssignment();
        }

        public object GetSourcePropertyValue(string propertyName)
        {
            return null;
        }

        public object GetDestinationPropertyValue(string propertyName)
        {
            return null;
        }

        public IResolutionContext Nested(IDescribeMappableProperty destinationProperty)
        {
            throw new DittoExecutionException("{0} is null. As such, a nested context is non-sensical.", SourceType);
        }

        public IResolutionContext Nested(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty)
        {
            return new NullSourceContext(sourceProperty,valueAssignments,invoke,activate);
        }
    }
}