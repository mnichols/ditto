using System.Collections.Generic;
using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class collection_mapping_tests:WithDebugging
    {
        private TestContextualizer contextualizer;

        public collection_mapping_tests()
        {
            contextualizer = new TestContextualizer();
        }
        
        [Fact]
        public void collections_of_components_are_mapped()
        {
            var componentConfig = new DestinationConfiguration(typeof (IntegerDest));
            componentConfig.From(typeof (IntegerSource));
            componentConfig.Bind();
            
            var modelConfig = new DestinationConfiguration(typeof (DestinationWithComponentArray));
            modelConfig.From(typeof (SourceWithComponentArray));
            modelConfig.Bind((ICreateExecutableMapping) componentConfig);
            modelConfig.Assert();
            var source = new SourceWithComponentArray()
                             {
                                 IntegerComponents =
                                     new[] {new IntegerSource() {AnInt = 1}, new IntegerSource() {AnInt = 3},}
                             };
            var dest = new DestinationWithComponentArray();
            var executable = modelConfig.CreateExecutableMapping(typeof (SourceWithComponentArray));
            executable.Execute(contextualizer.CreateContext(source,dest));
            dest.IntegerComponents.Length.should_be_equal_to(2);
        }
        [Fact]
        public void collections_of_components_are_mapped_with_list()
        {
            var componentConfig = new DestinationConfiguration(typeof(IntegerDest));
            componentConfig.From(typeof(IntegerSource));
            componentConfig.Bind();

            var modelConfig = new DestinationConfiguration(typeof(DestWithCollections));
            modelConfig.From(typeof(SourceWithCollections));
            modelConfig.Bind(componentConfig,modelConfig);
            modelConfig.Assert();
            var source = new SourceWithCollections()
            {
                ArrayOfIntegerComponents = new IntegerSource[0],
                ListOfIntegerComponents = new List<IntegerSource>{new IntegerSource() { AnInt = 1 }, new IntegerSource() { AnInt = 3 }, }
            };
            var dest = new DestWithCollections();
            var executable = modelConfig.CreateExecutableMapping(typeof(SourceWithCollections));
            executable.Execute(contextualizer.CreateContext(source, dest));
            dest.ListOfIntegerComponents.Count.should_be_equal_to(2);
        }
    }
}