using System;
using System.Collections;
using System.Collections.Generic;

namespace Ditto
{
    internal static class InternalCollectionExtensions
    {

        internal static void AddElement(this IList list,object value,int index)
        {
            if(list.GetType().IsArray)
            {
                ((Array)list).SetValue(value,index);
            }
            else
            {
                list.Add(value);
            }

        }

        internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}