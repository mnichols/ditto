using System;
using System.ComponentModel;
using Ditto.Internal;

namespace Ditto.Converters
{
    public class SystemConverter:IConvertValue
    {
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue &&
                   conversion.DestinationPropertyType is IConvertible &&
                   conversion.ValueType != conversion.DestinationPropertyType;
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            try
            {
                if (IsConvertible(conversion))
                    return conversion.Unconverted();
                var destPropType = conversion.DestinationPropertyType;
                if (destPropType.IsNullableType() && conversion.HasValue)
                    destPropType = new NullableConverter(destPropType).UnderlyingType;

                return conversion.Result(System.Convert.ChangeType(conversion.Value, destPropType));
            }
            catch(InvalidCastException)
            {
                //gulp
                return conversion.Unconverted();
            }

        }
    }
}