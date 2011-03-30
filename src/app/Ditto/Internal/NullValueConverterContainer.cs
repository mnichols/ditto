using System;
using System.Collections;
using System.Collections.Generic;

namespace Ditto.Internal
{
    public class NullValueConverterContainer:IValueConverterContainer
    {
        static NullValueConverterContainer()
        {
            instance=new NullValueConverterContainer();
        }

        private static NullValueConverterContainer instance;
        public static IValueConverterContainer Instance { get { return instance; } }
        private NullValueConverterContainer()
        {
        }

        private readonly List<IConvertValue> empty = new List<IConvertValue>();
        public IEnumerator<IConvertValue> GetEnumerator()
        {
            return empty.GetEnumerator();
        }

        public IValueConverterContainer FilteredBy(Func<IConvertValue, bool> targets)
        {
            return new NullValueConverterContainer();
        }

        public void AddConverter(IConvertValue converter)
        {
            //no op
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}