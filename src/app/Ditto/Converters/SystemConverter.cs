using System;
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
                return conversion.Result(System.Convert.ChangeType(conversion.Value, conversion.DestinationPropertyType));
            }
            catch(InvalidCastException ex)
            {
                //gulp
                return conversion.Unconverted();
            }

        }
    }
}