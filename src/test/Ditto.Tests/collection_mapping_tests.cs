using System;
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
        public void executing_mapping_on_collection_with_unmapped_elements_throws()
        {
//            container.Map(typeof(IntegerDest)).From(typeof(IntegerSource));
            container.Map(typeof(DestinationWithComponentArray)).From(typeof(SourceWithComponentArray));
            var bindable = container.ToBinding();
            bindable.Bind();
            bindable.Assert();
            var source = new SourceWithComponentArray()
            {
                IntegerComponents =
                    new[] { new IntegerSource() { AnInt = 1 }, new IntegerSource() { AnInt = 3 }, }
            };
            var dest = new DestinationWithComponentArray();
            var executable = bindable.CreateCommand(typeof(SourceWithComponentArray), typeof(DestinationWithComponentArray));
            Action executing=()=>executable.Map(source, dest);
            executing.should_throw_because<DittoExecutionException>("'Ditto.Tests.DestinationWithComponentArray:IntegerComponents' requires a collection configuration which was not provided. " 
                + Environment.NewLine 
                +"Be sure you have either provided configuration for the element type ('Ditto.Tests.IntegerDest'), or the collection.");
        }
        [Fact]
        public void collections_of_components_are_mapped()
        {
            container.Map(typeof (IntegerDest)).From(typeof (IntegerSource));
            container.Map(typeof (DestinationWithComponentArray)).From(typeof (SourceWithComponentArray));
            var bindable=container.ToBinding();
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
            var bindable = container.ToBinding();
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