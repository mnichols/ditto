using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class list_property_binder_tests
    {
        [Fact]
        public void it_should_determine_candidate_properties()
        {
            var modelConfig = new DestinationConfiguration(typeof(DestinationWithComponentArray), new TestDestinationConfigurationFactory());
            modelConfig.From(typeof (SourceWithComponentArray));
            var cfg = modelConfig.CreateBindableConfiguration();
            var binder = new ListPropertyBinder(null);
            var candidates = binder.GetCandidateDestinationProperties(cfg);
            candidates.should_have_item_matching(its => its.PropertyType == typeof (IntegerDest[]));
        }
    }
}