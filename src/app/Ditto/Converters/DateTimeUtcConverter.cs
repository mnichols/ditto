using System;
using Ditto.Internal;

namespace Ditto.Converters
{
    public class DateTimeUtcConverter:IConvertValue
    {
        private static Type[] dateTimeTypes=new []{typeof(DateTime),typeof(DateTime?)};
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue &&
                   conversion.ValueType.IsOneOf(dateTimeTypes) &&
                   conversion.DestinationPropertyType.IsOneOf(dateTimeTypes);
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            if (conversion.HasValue==false)
                return null;
            if(conversion.Is<DateTime?>())
            {
                return conversion.Result(conversion.Value.As<DateTime?>().Value.ToUniversalTime());
            }
            return conversion.Result(conversion.Value.As<DateTime>().ToUniversalTime());
        }
    }
}