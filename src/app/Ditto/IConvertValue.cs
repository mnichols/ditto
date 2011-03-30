using Ditto.Internal;

namespace Ditto
{
    public interface IConvertValue
    {
        bool IsConvertible(ConversionContext conversion);
        ConversionResult Convert(ConversionContext conversion);
    }
}