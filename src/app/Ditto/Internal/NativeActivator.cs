using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ditto.Internal
{
    public class NativeActivator : IActivator
    {
        public object CreateInstance(Type ofType)
        {
            return Activator.CreateInstance(ofType);
        }

        public object CreateCollectionInstance(Type collectionType, int length)
        {
            if (collectionType.IsArray)
                return Array.CreateInstance(collectionType.ElementType(), length);
            return CreateList(collectionType.ElementType());
        }

        private object CreateList(Type elementType)
        {
            Type destListType = typeof (List<>).MakeGenericType(elementType);
            return CreateCtor(destListType)();
        }

        internal static LateBoundCtor CreateCtor(Type type)
        {
            var ctorExpression =
                Expression.Lambda<LateBoundCtor>(Expression.Convert(Expression.New(type), typeof (object)));
            return ctorExpression.Compile();
        }

        internal delegate object LateBoundCtor();
    }
}