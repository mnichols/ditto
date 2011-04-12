using System;
using Ditto.Internal;
using Xunit;
using Fasterflect;

namespace Ditto.Tests
{
    public class interface_source_tests
    {

        [Fact]
        public void it_should_map_from_interface_source()
        {
            var cfg = new DestinationConfigurationContainer(null, new TestConfigurationFactory());
            cfg.Map<Destination>().From<IWorkOkay>();
            var binding = cfg.ToBinding();
            binding.Bind();
            binding.Assert();
            var cmd = binding.CreateCommand(typeof (SourceImplementation), typeof (Destination));
            var src = new SourceImplementation() {Name = "mikey"};
            var dest = new Destination();
            cmd.Map(src, dest);
            dest.Name.should_be_equal_to("mikey");


        }
        
        [Fact]
        public void it_should_cache_source_implementation_getter_invocations_after_first_call()
        {
            var cache = new Fasterflection();
            var cfg = new DestinationConfigurationContainer(null, new TestConfigurationFactory());
            cfg.Map<Destination>().From<IWorkOkay>();
            var binding = cfg.ToBinding();
            binding.Bind();
            binding.Assert();
            binding.Accept(new CacheInitializer(cache));
            var src = new SourceImplementation() {Name = "mikey"};
            cache.HasCachedGetterFor(typeof(SourceImplementation), "Name").should_be_false();
            var value=cache.GetValue("Name", src);
            cache.HasCachedGetterFor(typeof (SourceImplementation), "Name").should_be_true();
            value.should_be_equal_to("mikey");
        }
        [Fact]
        public void multiple_source_implementations_work()
        {
            var cfg = new DestinationConfigurationContainer(null, new TestConfigurationFactory());
            cfg.Map<DestinationWithSpecialSauce>().From<IWorkOkay,YetAnotherImplementation>();
            var binding = cfg.ToBinding();
            binding.Bind();
            binding.Assert();
            binding.Accept(new CacheInitializer(new Fasterflection()));

            var dest = new DestinationWithSpecialSauce();
            var cmd1 = binding.CreateCommand(typeof(SourceImplementation), typeof(DestinationWithSpecialSauce));
            var source1 = new SourceImplementation() { Name = "mikey" };
            cmd1.Map(source1, dest);
            dest.Name.should_be_equal_to("mikey");


            dest = new DestinationWithSpecialSauce();
            var cmd2 = binding.CreateCommand(typeof(YetAnotherImplementation), typeof(DestinationWithSpecialSauce));
            var source2 = new YetAnotherImplementation() { Name = "tasha" ,SpecialProperty = 3};
            cmd2.Map(source2, dest);
            dest.Name.should_be_equal_to("tasha");
            dest.SpecialProperty.should_be_equal_to(3);

        }
        public interface IWorkOkay
        {
            string Name { get; set; }
        }
        public class SourceImplementation:IWorkOkay
        {
            public string Name { get; set; }
        }
        public class YetAnotherImplementation : IWorkOkay
        {
            public string Name { get; set; }
            public int SpecialProperty { get; set; }
        }
        public class Destination
        {
            public string Name { get; set; }
            
        }
        public class DestinationWithSpecialSauce
        {

            public string Name { get; set; }
            public int SpecialProperty { get; set; }
        }
        
    }
}