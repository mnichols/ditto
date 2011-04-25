using System;

namespace Ditto.Internal
{
    public class ValueAssignment : IAssignValue
    {
        private readonly object destination;
        private readonly IDescribeMappableProperty destinationProperty;
        private readonly IInvoke invoke;
        private readonly IValueConverterContainer valueConverters;

        public ValueAssignment(object destination, IDescribeMappableProperty destinationProperty,
                               IValueConverterContainer valueConverters, IInvoke invoke)
        {
            if (destinationProperty == null)
                throw new ArgumentNullException("destinationProperty");
            this.destination = destination;
            this.destinationProperty = destinationProperty;
            this.invoke = invoke;
            this.valueConverters = valueConverters ?? NullValueConverterContainer.Instance;
        }

        public void SetValue(Result value)
        {
            if (value.IsResolved == false)
                return;
            var assignable = new AssignableValue(value.Value, destinationProperty, valueConverters);

            invoke.SetValue(assignable, destination);
        }
    }
}