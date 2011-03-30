using System.Collections.Generic;
using Fasterflect;
using Xunit;

namespace Ditto.Tests
{
    public class fasterflect_spike_tests
    {
        [Fact]
        public void deep_cloning_does_not_copy_references()
        {
            var sampleClass = new SampleClass() {Name = "mikey", Other = new OtherClass() {Name = "natasha"}};
            var clone = sampleClass.DeepClone();
            clone.Name.should_be_equal_to("mikey");
            clone.Other.should_not_be_null();
            clone.Other.Name.should_be_equal_to("natasha");
            Assert.NotSame(clone.Other, sampleClass.Other);
        }
        [Fact]
        public void deep_cloning_is_okay_with_lists()
        {
            var sampleClass = new SampleClass() { Name = "mikey", Other = new OtherClass() { Name = "natasha" } ,Strings = new List<string>{"chaya","joshua"}};
            var clone = sampleClass.DeepClone();
            clone.Name.should_be_equal_to("mikey");
            clone.Other.should_not_be_null();
            clone.Other.Name.should_be_equal_to("natasha");
            clone.Strings.should_contain_equal_elements_as(new[]{"chaya","joshua"});
            Assert.NotSame(clone.Other, sampleClass.Other);
            Assert.NotSame(clone.Strings, sampleClass.Strings);
        }
        [Fact]
        public void deep_cloning_is_okay_with_dictionaries()
        {
            var sampleClass = new SampleClassWithDic() { String2Complex = new Dictionary<string, OtherClass>
                                                                              {
                                                                                  {"one",new OtherClass{Name = "chaya"}},
                                                                                  {"two",new OtherClass{Name = "joshua"}},
                                                                              }};
            var clone = sampleClass.DeepClone();
            Assert.NotSame(clone.String2Complex,sampleClass.String2Complex);
            clone.String2Complex["one"].Name.should_be_equal_to("chaya");
            clone.String2Complex["two"].Name.should_be_equal_to("joshua");
            
        }
        public class SampleClass
        {
            public string Name { get; set; }
            public OtherClass Other { get; set; }
            public List<string> Strings { get; set; }
        }
        public class OtherClass
        {
            public string Name { get; set; }
        }
        public class SampleClassWithDic
        {
            public Dictionary<string, OtherClass> String2Complex{ get; set; }
        }
    }
}