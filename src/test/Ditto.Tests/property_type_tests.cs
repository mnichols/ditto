using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class property_type_tests
    {
        [Fact]
        public void it_should_get_destination_property_for_underlying_element()
        {
            var dest = MappableProperty.For<DestinationWithComponentArray>(d => d.IntegerComponents);
            dest.PropertyType.should_be_equal_to(typeof(IntegerDest[]));
            var el=dest.ElementAt(2);
            el.PropertyType.should_be_equal_to(typeof(IntegerDest));
            el.Index.should_be_equal_to(2);

        }
    }
}