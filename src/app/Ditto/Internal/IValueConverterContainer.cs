using System;
using System.Collections.Generic;

namespace Ditto.Internal
{
    public interface IValueConverterContainer:IEnumerable<IConvertValue>
    {
        IValueConverterContainer FilteredBy(Func<IConvertValue, bool> targets);
        void AddConverter(IConvertValue converter);
    }
}