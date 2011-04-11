using Ditto.Internal;
using Xunit;

namespace Ditto.Tests
{
    public class list_property_binder_tests
    {
        private TestDestinationConfigurationFactory configFactory;

        public list_property_binder_tests()
        {
            configFactory = new TestDestinationConfigurationFactory();
        }
        [Fact]
        public void it_should_determine_candidate_properties()
        {
            var modelConfig = new DestinationConfiguration(typeof(DestinationWithComponentArray), configFactory);
            modelConfig.From(typeof (SourceWithComponentArray));
            var cfg = configFactory.CreateBindableConfiguration(modelConfig.ToSnapshot());
            var binder = new ListPropertyBinder(null);
            var candidates = binder.GetCandidateDestinationProperties(cfg);
            candidates.should_have_item_matching(its => its.PropertyType == typeof (IntegerDest[]));
        }
    }
}