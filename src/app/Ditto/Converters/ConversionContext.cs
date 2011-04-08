using System;

namespace Ditto.Converters
{
    public class ConversionContext
    {
        public ConversionContext(Type destinationPropertyType, object value)
        {
            DestinationPropertyType = destinationPropertyType;
            Value = value;
        }

        public Type DestinationPropertyType { get; private set; }

        public bool HasValue
        {
            get { return Value != null; }
        }

        public object Value { get; private set; }

        public Type ValueType
        {
            get
            {
                if (HasValue == false)
                    return null;
                return Value.GetType();
            }
        }

        public bool Is<T>()
        {
            return Value != null && typeof (T).IsInstanceOfType(Value);
        }

        public ConversionResult Result(object value)
        {
            return new ConversionResult(DestinationPropertyType, value);
        }

        public ConversionResult Unconverted()
        {
            return new Unconverted(DestinationPropertyType, Value);
        }
    }
}