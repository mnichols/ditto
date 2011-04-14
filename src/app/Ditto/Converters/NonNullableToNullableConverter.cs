using System.ComponentModel;
using Ditto.Internal;

namespace Ditto.Converters
{
    public class NonNullableToNullableConverter : IConvertValue
    {
        public bool IsConvertible(ConversionContext conversion)
        {
            var convertible=conversion.HasValue &&
                   conversion.ValueType.IsNullableType() == false &&
                   conversion.DestinationPropertyType.IsNullableType();
            if(convertible)
            {
                var cnv = new NullableConverter(conversion.DestinationPropertyType);
                return conversion.ValueType == cnv.UnderlyingType;
            }
            return false;
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            return conversion.Unconverted();
        }

    }
}