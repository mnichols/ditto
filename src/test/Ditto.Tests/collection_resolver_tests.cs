using System.Collections.Generic;
using Ditto.Internal;
using Ditto.Resolvers;
using Xunit;

namespace Ditto.Tests
{
    public class collection_resolver_tests
    {
        private TestContextualizer contextualizer;
        private ICreateExecutableMapping integerComponentElementConfig;

        public collection_resolver_tests()
        {
            var factory = new TestConfigurationFactory();
            var cfg = new DestinationConfiguration(typeof (IntegerDest));
            cfg.From(typeof (IntegerSource));
            integerComponentElementConfig = factory.CreateBindableConfiguration(cfg.TakeSnapshot());
            contextualizer = new TestContextualizer();
        }
       
        [Fact]
        public void it_should_resolve_to_list_from_list()
        {
            var resolver = new ListResolver(MappableProperty.For<SourceWithCollections>(s => s.ListOfIntegerComponents),integerComponentElementConfig, new Fasterflection());
            var source = new SourceWithCollections()
            {
                ListOfIntegerComponents =
                    new List<IntegerSource> { new IntegerSource { AnInt = 1 }, new IntegerSource() { AnInt = 4 } }
            };


            var destination = new DestWithCollections();
            var result = resolver.TryResolve(contextualizer.CreateContext(source, destination), MappableProperty.For<DestWithCollections>(its=>its.ListOfIntegerComponents));
            result.Value.should_be_a_type_of<List<IntegerDest>>();
            ((List<IntegerDest>)result.Value)[0].AnInt.should_be_equal_to(1);
            ((List<IntegerDest>)result.Value)[1].AnInt.should_be_equal_to(4);
        }

        [Fact]
        public void it_should_resolve_to_array_from_array()
        {
            var resolver = new ListResolver(MappableProperty.For<SourceWithCollections>(s => s.ArrayOfIntegerComponents),integerComponentElementConfig, new Fasterflection());
            var source = new SourceWithCollections()
                             {
                                 ArrayOfIntegerComponents = 
                                     new[] {new IntegerSource {AnInt = 1}, new IntegerSource() {AnInt = 4}}
                             };


            var destination = new DestWithCollections();
            var result = resolver.TryResolve(contextualizer.CreateContext(source, destination), MappableProperty.For<DestWithCollections>(its => its.ArrayOfIntegerComponents));
            result.Value.should_be_a_type_of<IntegerDest[]>();
            ((IntegerDest[])result.Value)[0].AnInt.should_be_equal_to(1);
            ((IntegerDest[])result.Value)[1].AnInt.should_be_equal_to(4);
        }
        [Fact]
        public void it_should_resolve_from_array_to_list()
        {
            var resolver = new ListResolver(MappableProperty.For<SourceWithCollections>(s => s.ArrayOfIntegerComponents),integerComponentElementConfig, new Fasterflection());
            var source = new SourceWithCollections()
            {
                ArrayOfIntegerComponents =
                    new[] { new IntegerSource { AnInt = 1 }, new IntegerSource() { AnInt = 4 } }
            };


            var destination = new DestWithCollections();
            var result = resolver.TryResolve(contextualizer.CreateContext(source, destination), MappableProperty.For<DestWithCollections>(its => its.ListOfIntegerComponents));
            result.Value.should_be_a_type_of<List<IntegerDest>>();
            ((List<IntegerDest>)result.Value)[0].AnInt.should_be_equal_to(1);
            ((List<IntegerDest>)result.Value)[1].AnInt.should_be_equal_to(4);
        }
        [Fact]
        public void it_should_resolve_from_list_to_array()
        {
            var resolver = new ListResolver(MappableProperty.For<SourceWithCollections>(s => s.ListOfIntegerComponents),integerComponentElementConfig, new Fasterflection());
            var source = new SourceWithCollections()
            {
                ListOfIntegerComponents =
                    new List<IntegerSource> { new IntegerSource { AnInt = 1 }, new IntegerSource() { AnInt = 4 } }
            };


            var destination = new DestWithCollections();
            var result = resolver.TryResolve(contextualizer.CreateContext(source, destination), MappableProperty.For<DestWithCollections>(its => its.ArrayOfIntegerComponents));
            result.Value.should_be_a_type_of<IntegerDest[]>();
            ((IntegerDest[])result.Value)[0].AnInt.should_be_equal_to(1);
            ((IntegerDest[])result.Value)[1].AnInt.should_be_equal_to(4);
        }
    }
    public class SourceWithCollections
    {
        public SourceWithCollections()
        {
            ListOfIntegerComponents=new List<IntegerSource>();
        }
        public IntegerSource[] ArrayOfIntegerComponents { get; set; }
        public List<IntegerSource> ListOfIntegerComponents { get; set; }
    }
    public class DestWithCollections
    {
        public DestWithCollections()
        {
            ListOfIntegerComponents=new List<IntegerDest>();
        }
        public IntegerDest[] ArrayOfIntegerComponents { get; set; }
        public List<IntegerDest> ListOfIntegerComponents { get; set; }
    }
}