using System;

namespace Ditto.Converters
{
    public class Unconverted : ConversionResult
    {
        public Unconverted(Type destinationPropertyType, object value) : base(destinationPropertyType, value)
        {
        }
    }
}