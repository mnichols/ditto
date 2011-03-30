using Ditto.Internal;

namespace Ditto.Converters
{
    internal class NullConverter:IConvertValue
    {
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue==false;
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            return conversion.Result(conversion.DestinationPropertyType.DefaultValue());
        }
    }
}