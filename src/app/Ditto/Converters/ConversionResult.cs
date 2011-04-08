using System;

namespace Ditto.Converters
{
    public class ConversionResult
    {
        public Type DestinationPropertyType { get; private set; }
        public object Value { get; private set; }

        public ConversionResult(Type destinationPropertyType, object value)
        {
            DestinationPropertyType = destinationPropertyType;
            Value = value;
        }
        
    }
}