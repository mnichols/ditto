using System;
using System.Collections;
using Ditto.Internal;

namespace Ditto.Converters
{
    public class ValueMustMatchDestinationTypeConverter : IConvertValue
    {
        private readonly IActivator activator;

        public ValueMustMatchDestinationTypeConverter(IActivator activator)
        {
            this.activator = activator;
        }

        private ConversionResult TryListConversion(ConversionContext context)
        {
            var collectionTypeSpec = new SupportedCollectionTypeSpecification();
            if (collectionTypeSpec.IsSatisfiedBy(context.DestinationPropertyType) == false || collectionTypeSpec.IsSatisfiedBy(context.Value)==false)
                return context.Unconverted();
            
            var destColl = (IList)activator.CreateCollectionInstance(context.DestinationPropertyType,collectionTypeSpec.GetLength(context.Value));
            var destElement = context.DestinationPropertyType.ElementType();
            var srcList = (IEnumerable) context.Value;
            var enumerator = srcList.GetEnumerator();
            int index = 0;
            while(enumerator.MoveNext())
            {
                object element = enumerator.Current ?? destElement.DefaultValue();
                destColl.AddElement(element,index++);
            }
            return context.Result(destColl);
        }
        private static ConversionResult TryNullableConversion(ConversionContext context)
        {
            if (context.HasValue== false|| context.ValueType.IsNullableType() == false)
                return context.Unconverted();
            return  new NullableToNonNullableConverter().Convert(context);
        }
        private static ConversionResult TrySystemConversion(ConversionContext context)
        {
            return new SystemConverter().Convert(context);
        }
        public bool IsConvertible(ConversionContext conversion)
        {
            return conversion.HasValue;
        }

        public ConversionResult Convert(ConversionContext conversion)
        {
            if (conversion.HasValue == false || conversion.ValueType == conversion.DestinationPropertyType)
                return conversion.Result(conversion.Value);

            ConversionResult result = conversion.Unconverted();
            var attempts = new Func<ConversionContext, ConversionResult>[]
                                                                       {
                                                                           TryListConversion, 
                                                                           TryNullableConversion,
                                                                           TrySystemConversion
                                                                       };
            foreach (var attempt in attempts)
            {
                result = attempt(conversion);
                if(result is Unconverted)
                    continue;
                return result;
            }

            return result;
        }
    }
}