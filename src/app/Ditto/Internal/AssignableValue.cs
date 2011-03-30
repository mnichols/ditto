using System;
using System.Linq;

namespace Ditto.Internal
{
    public class AssignableValue
    {
        private readonly IDescribeMappableProperty destinationProperty;
        private readonly IValueConverterContainer valueConverters;
        private object destinationValue;
        private object originalValue;

        public AssignableValue(object value, IDescribeMappableProperty destinationProperty, IValueConverterContainer valueConverters)
        {
            if (destinationProperty == null)
                throw new ArgumentNullException("destinationProperty");
            this.valueConverters = valueConverters ?? NullValueConverterContainer.Instance;
            this.destinationProperty = destinationProperty;
            this.originalValue = value;
            destinationValue = value;
            TryConvert();
        }

        public object DestinationValue
        {
            get { return destinationValue; }
        }

        public string PropertyName
        {
            get { return destinationProperty.Name; }
        }

        private void TryConvert()
        {
            var conversionContext = new ConversionContext(destinationProperty.PropertyType, destinationValue);
            var converter = valueConverters.FirstOrDefault(it => it.IsConvertible(conversionContext));
            if (converter != null)
            {
                destinationValue = converter.Convert(conversionContext).Value;
            }
            
        }

        

        public override string ToString()
        {
            return destinationProperty + " , SourceValue =  [" + originalValue + "], DestinationValue = [" +
                   DestinationValue + "]";
        }
    }
}