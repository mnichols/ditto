using System;
using Ditto.Converters;
using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class value_converter_tests
    {
        private Destination dest;
        private Source src;

        private IResolutionContext Contextualize(params IConvertValue[] testConverters)
        {
            var reflection = new Fasterflection();
            var converters = new DefaultValueConverterContainer(reflection);
            foreach (var testConverter in testConverters)
            {
                converters.AddConverter(testConverter);
            }
            return new DefaultResolutionContext(src, dest, null, new ValueAssignments(converters, reflection ),reflection);
        }   
        public value_converter_tests()
        {
            src = new Source();
            dest = new Destination();
        }

        [Fact]
        public void it_can_map_numeric_to_string()
        {
            var context=Contextualize(new SimpleIntToStringConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.AnInt));
            assignment.SetValue(new Result(true,3));
            dest.AnInt.should_be_equal_to("3");
        }

        [Fact]
        public void it_can_map_string_to_datetime()
        {
            var context = Contextualize(new StringToDateTimeConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.AnDateTime));
            assignment.SetValue(new Result(true,new DateTime(2009, 8, 1, 0, 0, 0).ToString()));
            dest.AnDateTime.should_be_equal_to(new DateTime(2009, 8, 1, 0, 0, 0));
        }
        [Fact]
        public void it_can_map_nullable_to_nonnullable_types()
        {
            var context = Contextualize(new NullableToNonNullableConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.Int));
            int? nint = 4;
            assignment.SetValue(new Result(true,nint));
            dest.Int.should_be_equal_to(4);

        }
        [Fact]
        public void it_can_map_nullable_double_to_nonnullable_types_with_default_value()
        {
            var context = Contextualize(new NullableToNonNullableConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.Double));
            double? ndouble = null;
            assignment.SetValue(new Result(true, ndouble));
            dest.Double.should_be_equal_to(0);

        }
        [Fact]
        public void it_can_map_nonnullable_to_nullable_types()
        {
            var context = Contextualize(new NonNullableToNullableConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.NInt));
            int iint = 4;
            assignment.SetValue(new Result(true, iint));
            dest.NInt.should_be_equal_to(4);

        }
        [Fact]
        public void it_can_map_unsigned_values_to_their_signed_equivalent()
        {

            var context = Contextualize(new SystemConverter());
            var assignment = context.BuildValueAssignment(MappableProperty.For<Destination>(d => d.Int));
            uint unsigned = 42;
            assignment.SetValue(new Result(true,unsigned));
            dest.Int.should_be_equal_to(42);
        }
        
        public class Destination
        {
            public DateTime AnDateTime { get; set; }
            public string AnInt { get; set; }
            public int Int { get; set; }
            public int? NInt { get; set; }
            public double Double { get; set; }
            
        }

        public class SimpleIntToStringConverter : IConvertValue
        {
            public bool IsConvertible(ConversionContext conversion)
            {
                return conversion.Value is Int32 &&
                       conversion.DestinationPropertyType == typeof (String);
            }

            public ConversionResult Convert(ConversionContext conversion)
            {
                return conversion.Result(conversion.HasValue?conversion.Value.ToString():null);
            }

        }

        public class Source
        {
            public string AnDateTime { get; set; }
            public int AnInt { get; set; }
            public uint AnUint { get; set; }
            public double? NDouble { get; set; }
        }

        public class StringToDateTimeConverter : IConvertValue
        {
            public bool IsConvertible(ConversionContext conversion)
            {
                DateTime ignore;
                return (conversion.HasValue &&
                        DateTime.TryParse(conversion.Value.ToString(), out ignore) &&
                        (conversion.DestinationPropertyType == typeof (DateTime) || conversion.DestinationPropertyType == typeof (DateTime?)));
            }

            public ConversionResult Convert(ConversionContext conversion)
            {
                return conversion.Result(DateTime.Parse(conversion.Value.ToString()));
            }

        }
    }
}