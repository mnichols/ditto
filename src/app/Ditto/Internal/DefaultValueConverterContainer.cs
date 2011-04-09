using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ditto.Converters;

namespace Ditto.Internal
{
    public class DefaultValueConverterContainer : IValueConverterContainer
    {
        private readonly IActivate activate;

        public DefaultValueConverterContainer(IActivate activate)
        {
            this.activate = activate;
            /*always support null conversion*/
            converters.Push(new NullConverter());

            converters.Push(new ValueMustMatchDestinationTypeConverter(activate));
            
        }

        private readonly Stack<IConvertValue> converters = new Stack<IConvertValue>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IValueConverterContainer FilteredBy(Func<IConvertValue, bool> targets)
        {
            var container = new DefaultValueConverterContainer(activate);
            container.AddRange(converters.Where(targets));
            return container;
        }
        private void AddRange(IEnumerable<IConvertValue> addConverters)
        {
            foreach (var addConverter in addConverters)
            {
                converters.Push(addConverter);
            }
        }
        public IEnumerator<IConvertValue> GetEnumerator()
        {
            return converters.GetEnumerator();
        }

        public void AddConverter(IConvertValue converter)
        {
            if(converter==null)
                throw new ArgumentNullException("converter");

            converters.Push(converter);
        }
    }
}