using System;
using Ditto.Internal;
using Xunit;

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
        public interface IWorkOkay
        {
            string Name { get; set; }
        }
        public class SourceImplementation:IWorkOkay
        {
            public string Name { get; set; }
        }
        public class Destination
        {
            public string Name { get; set; }
        }
    }
}