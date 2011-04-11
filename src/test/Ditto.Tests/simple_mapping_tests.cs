using System;
using System.Collections.Generic;
using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class simple_mapping_tests
    {
    
        [Fact]
        public void it_should_map_props_of_same_name()
        {
            var src = new SimpleSource() {Name = "mikey"};
            var cfg = new DestinationConfiguration(typeof(SimpleDestination), new TestConfigurationFactory());
            cfg.From(typeof (SimpleSource));

            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable,new TestContextualizer());
            var output = (SimpleDestination) mapper.From(src);
            output.Name.should_be_equal_to("mikey");
        }

        [Fact]
        public void it_should_handle_collections_natively()
        {
            var src = new CollectionSource {Name = "mikey", List = new List<string> {"one", "two"}};
            var cfg = new DestinationConfiguration(typeof(CollectionDestination), new TestConfigurationFactory());
            cfg.From(typeof (CollectionSource));
            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable, new TestContextualizer());
            var dest = (CollectionDestination)mapper.From(src);
            dest.Name.should_be_equal_to("mikey");
            dest.List.should_not_be_null();
            dest.List.Count.should_be_equal_to(2);
            dest.List[0].should_be_equal_to("one");
            dest.List[1].should_be_equal_to("two");
        }

        [Fact]
        public void it_can_transparently_convert_values()
        {
            var src = new IntegerSource() {AnInt = 3};
            var cfg = new DestinationConfiguration(typeof(IntegerDest), new TestConfigurationFactory());
            cfg.From(typeof (IntegerSource));
            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable, new TestContextualizer());
            var dest = (IntegerDest)mapper.From(src);
            dest.AnInt.should_be_equal_to(3);
        }
        
        [Fact]
        public void it_can_map_to_instance()
        {
            var src = new OnlyCollectionSource() { List = new List<string> { "blah", "goo" } };
            var cfg = new DestinationConfiguration(typeof(MultiDestination), new TestConfigurationFactory());
            cfg.From(typeof (OnlyCollectionSource));
            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable, new TestContextualizer());
            var dest = new MultiDestination(){Name = "ignore"};

            var output = (MultiDestination)mapper.From(src, dest);
            output.Name.should_be_equal_to("ignore");
            output.List.Count.should_be_equal_to(2);
            output.List[0].should_be_equal_to("blah");
            output.List[1].should_be_equal_to("goo");
        }

        [Fact]
        public void it_can_redirect_property_values()
        {
            var cfg = new DestinationConfiguration<TypicalViewModel>(new TestConfigurationFactory());
            cfg.From(typeof (TypicalEvent))
                .Redirecting<TypicalEvent>(its => its.Id, m => m.SomeId);
            Guid eventId=Guid.NewGuid();
            var src = new TypicalEvent()
                             {
                                 Id = eventId,
                                 Name = "bob"
                             };
            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable, new TestContextualizer());
            var output = (TypicalViewModel) mapper.From(src);
            output.Name.should_be_equal_to("bob");
            output.SomeId.should_be_equal_to(eventId);
        }

        
        [Fact]
        public void it_can_map_list_of_nullable_double_to_nonnullable_double_list()
        {
            var cfg = new DestinationConfiguration(typeof(CollectionDestination), new TestConfigurationFactory());
            cfg.From(typeof (CollectionSource));
            var src = new CollectionSource() {MyDoubles = new List<double?> {2.1, null, 3.2}};
            var executable = cfg.ToExecutable(src.GetType());
            var mapper = new DefaultMapCommand(executable, new TestContextualizer());
            var output = (CollectionDestination) mapper.From(src);
            output.MyDoubles.Count.should_be_equal_to(3);
            output.MyDoubles[0].should_be_equal_to(2.1);
            output.MyDoubles[1].should_be_equal_to(0);
            output.MyDoubles[2].should_be_equal_to(3.2);
        }
    }


    public class CollectionSource
    {
        public List<string> List { get; set; }
        public string Name { get; set; }
        public List<double?> MyDoubles { get; set; }
    }

    public class CollectionDestination
    {
        public List<string> List { get; set; }
        public string Name { get; set; }
        public List<double> MyDoubles { get; set; }
    }


    public class IntegerSource
    {
        public int AnInt { get; set; }
    }

    public class IntegerDest
    {
        public int? AnInt { get; set; }
    }

    public class SimpleSource
    {
        public string Name { get; set; }
    }

    public class SimpleDestination
    {
        public string Name { get; set; }
    }
    public class OnlyCollectionSource
    {
        public List<string> List { get; set; }
    }
    public class MultiDestination
    {
        public string Name { get; set; }
        public List<string> List { get; set; }
    }
}