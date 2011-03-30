using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ditto.Tests
{
    public static class spec_extensions
    {
        public static void should_match<T>(this object item, Predicate<T> match)
        {
            Assert.IsType<T>(item);
            Assert.True(match.Invoke((T)item), "Item does not match");
        }
        public static void should_have_item_matching<T>(this IEnumerable<T> items, Func<T, bool> match)
        {
            items.Any(match).should_be_true();
        }
        
        public static void should_be_null<T>(this T obj)
        {
            Assert.Null(obj);
        }

        public static void should_not_be_null<T>(this T obj)
        {
            Assert.NotNull(obj);
        }
        
        
        public static void should_be_like(this string output, string expect)
        {
            //			var expectX = expect.Replace("/*<![CDATA[*/", "").Replace("});/*]]>*/", "").Replace(Environment.NewLine, "");
            //			var outputX = output.Replace("/*<![CDATA[*/", "").Replace("});/*]]>*/", "").Replace(Environment.NewLine, "");
            Assert.Equal(expect, output, StringComparer.InvariantCultureIgnoreCase);
        }
        public static void should_be_true(this bool isIt)
        {
            Assert.True(isIt);
        }
        public static void should_be_false(this bool isIt)
        {
            Assert.False(isIt);
        }

        public static void should_be_equal_to<T>(this T actual, T expect)
        {
            Assert.Equal(expect, actual);
        }
        public static void should_not_be_equal_to<T>(this T expect, T actual)
        {
            Assert.NotEqual(actual, expect);
        }
        public static void should_be_same_as<T>(this T expect, T actual)
        {
            Assert.NotNull(expect);
            Assert.NotNull(actual);
            Assert.Same(expect, actual);
        }
        public static void should_be_a_type_of<T>(this object expect)
        {
            Assert.IsType<T>(expect);
        }
        
        public static void should_contain_equal_elements_as<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            Assert.True(first.SequenceEqual(second), "The two lists are not sequence equal.");
        }

        
        public static void should_throw_an<ExceptionType>(this Action work_to_perform) where ExceptionType : Exception
        {
            Assert.Throws<ExceptionType>(() => work_to_perform());
        }
        public static void should_throw_an<ExceptionType>(this Action work_to_perform, Func<ExceptionType, bool> where) where ExceptionType : Exception
        {
            var ex = Assert.Throws<ExceptionType>(() => work_to_perform());
            Assert.True(where(ex), typeof(ExceptionType) + " expectations were not met");

        }
        public static void should_throw_because<ExceptionType>(this Action work_to_perform, string messageIs) where ExceptionType : Exception
        {
            var ex = Assert.Throws<ExceptionType>(() => work_to_perform());
            Assert.Equal(messageIs, ex.Message, StringComparer.InvariantCultureIgnoreCase);

        }
        public static void should_not_throw_an<ExceptionType>(this Action work_to_perform) where ExceptionType : Exception
        {
            Assert.DoesNotThrow(() => work_to_perform());
        }   
    }
}