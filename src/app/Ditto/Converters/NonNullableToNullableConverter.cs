using Ditto.Internal;

namespace Ditto.Converters
{
    public class NonNullableToNullableConverter : IConvertValue
    {
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue &&
                   conversion.ValueType.IsNullableType() == false &&
                   conversion.DestinationPropertyType.IsNullableType();
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            return conversion.Unconverted();
        }

    }
}