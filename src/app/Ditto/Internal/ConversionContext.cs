using System;

namespace Ditto.Internal
{
    public class ConversionContext
    {
        public Type DestinationPropertyType { get; private set; }
        public object Value { get; private set; }

        public ConversionContext(Type destinationPropertyType, object value)
        {
            DestinationPropertyType = destinationPropertyType;
            Value = value;
        }

        public bool Is<T>()
        {
            return Value != null && typeof (T).IsInstanceOfType(Value);
        }
        public ConversionResult Result(object value)
        {
            return new ConversionResult(this.DestinationPropertyType, value);
        }

        public bool HasValue
        {
            get { return Value != null; }
        }

        public Type ValueType
        {
            get
            {
                if (HasValue == false)
                    return null;
                return Value.GetType();
            }
        }
        public ConversionResult Unconverted()
        {
            return new Unconverted(this.DestinationPropertyType,this.Value);
        }
    }
}