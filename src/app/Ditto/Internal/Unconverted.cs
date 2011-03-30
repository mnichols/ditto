using System;

namespace Ditto.Internal
{
    public class Unconverted : ConversionResult
    {
        public Unconverted(Type destinationPropertyType, object value) : base(destinationPropertyType, value)
        {
        }
    }
}