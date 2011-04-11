using System.Collections.Generic;
using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class collection_mapping_tests:WithDebugging
    {
        private DestinationConfigurationContainer container;

        public collection_mapping_tests()
        {
            container = new DestinationConfigurationContainer(null, new TestConfigurationFactory());
        }
        
        [Fact]
        public void collections_of_components_are_mapped()
        {
            container.Map(typeof (IntegerDest)).From(typeof (IntegerSource));
            container.Map(typeof (DestinationWithComponentArray)).From(typeof (SourceWithComponentArray));
            var bindable=container.ToBindable();
            bindable.Bind();
            bindable.Assert();
            var source = new SourceWithComponentArray()
                             {
                                 IntegerComponents =
                                     new[] {new IntegerSource() {AnInt = 1}, new IntegerSource() {AnInt = 3},}
                             };
            var dest = new DestinationWithComponentArray();
            var executable = bindable.CreateCommand(typeof (SourceWithComponentArray), typeof (DestinationWithComponentArray));
            executable.Map(source, dest);
            dest.IntegerComponents.Length.should_be_equal_to(2);
        }
        [Fact]
        public void collections_of_components_are_mapped_with_list()
        {
            container.Map(typeof (IntegerDest)).From(typeof (IntegerSource));
            container.Map(typeof (DestWithCollections)).From(typeof (SourceWithCollections));
            var bindable = container.ToBindable();
            bindable.Bind();
            bindable.Assert();
            var source = new SourceWithCollections()
            {
                ArrayOfIntegerComponents = new IntegerSource[0],
                ListOfIntegerComponents = new List<IntegerSource>{new IntegerSource() { AnInt = 1 }, new IntegerSource() { AnInt = 3 }, }
            };
            var dest = new DestWithCollections();

            var executable = bindable.CreateCommand(typeof (SourceWithCollections), typeof (DestWithCollections));
            executable.Map(source, dest);
            dest.ListOfIntegerComponents.Count.should_be_equal_to(2);
        }
    }
}