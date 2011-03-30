using System;

namespace Ditto.Internal
{
    public class ValueAssignment : IAssignValue
    {
        private readonly object destination;
        private readonly IDescribeMappableProperty destinationProperty;
        private readonly IReflection reflection;
        private readonly IValueConverterContainer valueConverters;

        public ValueAssignment(object destination, IDescribeMappableProperty destinationProperty,
                               IValueConverterContainer valueConverters, IReflection reflection)
        {
            if (destinationProperty == null)
                throw new ArgumentNullException("destinationProperty");
            this.destination = destination;
            this.destinationProperty = destinationProperty;
            this.reflection = reflection;
            this.valueConverters = valueConverters ?? NullValueConverterContainer.Instance;
        }

        public void SetValue(Result value)
        {
            if (value.IsResolved == false)
                return;
            var assignable = new AssignableValue(value.Value, destinationProperty, valueConverters);
            reflection.SetValue(assignable, destination);
        }
    }
}