using System;
using System.ComponentModel;
using Ditto.Internal;

namespace Ditto.Converters
{
    public class NullableToNonNullableConverter:IConvertValue
    {
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue &&
                conversion.ValueType.IsNullableType() &&
                   (conversion.DestinationPropertyType.IsNullableType() == false);
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            if (conversion.HasValue==false)
                return conversion.Unconverted();
            
            var converter=new NullableConverter(typeof(Nullable<>).MakeGenericType(conversion.GetType()));
            return conversion.Result(converter.ConvertTo(conversion, converter.UnderlyingType));
        }

    }
}