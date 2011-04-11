using System;
using Ditto.Criteria;
using Ditto.Internal;
using Ditto.Resolvers;
using Xunit;

namespace Ditto.Tests
{
    public class conventional_mapping_tests
    {
        private TestContextualizer contextualizer;
        private TestConfigurationFactory bindableFactory;

        public conventional_mapping_tests()
        {
            contextualizer = new TestContextualizer();
            bindableFactory = new TestConfigurationFactory();
        }
        [Fact]
        public void it_should_include_conventions_in_validation()
        {

            var cfg = new DestinationConfiguration(typeof(SystemPerson), new TestConfigurationFactory());
            cfg.From(typeof (PersonalInfo), typeof (Parents))
                .ApplyingConvention(new PropertyNameCriterion("SystemId"), new IgnoreResolver());

            var bindable = bindableFactory.CreateBindableConfiguration(cfg.ToSnapshot());
            Action validation = bindable.Assert;
            validation.should_not_throw_an<MappingConfigurationException>();
        }
        [Fact]
        public void it_should_apply_convention_during_mapping()
        {
            var src1 = new PersonalInfo() { Age = 3, Name = "mikey" };
            var destination = new SystemPerson(){MothersName = "ignoremeplease"};
            Guid systemId=Guid.NewGuid();
            var cfg = new DestinationConfiguration<SystemPerson>(new TestConfigurationFactory());
            cfg.From(typeof(PersonalInfo), typeof(Parents))
                .ApplyingConvention(new StaticValueResolver(systemId), m=>m.SystemId);

            var bindable = bindableFactory.CreateBindableConfiguration(cfg.ToSnapshot());
//            bindable.Bind();
//            Action validation = bindable.Assert;
//            validation.should_not_throw_an<MappingConfigurationException>();
            var executable = bindable.CreateExecutableMapping(typeof (PersonalInfo));
            
            executable.Execute(contextualizer.CreateContext(src1,destination));
            destination.Age.should_be_equal_to(3);
            destination.MothersName.should_be_equal_to("ignoremeplease");
            destination.SystemId.should_be_equal_to(systemId);
            destination.Name.should_be_equal_to("mikey");
        }
        [Fact]
        public void it_should_give_priority_to_manually_configured_mappings()
        {
            var src1 = new PersonalInfo() { Age = 3, Name = "mikey" };
            var destination = new SystemPerson() { MothersName = "ignoremeplease" };
            Guid systemId = Guid.NewGuid();
            Guid myManualSystemId = new Guid("EE99B786-B3A2-426A-BA46-53B990383DA1");
            var cfg = new DestinationConfiguration<SystemPerson>(new TestConfigurationFactory());
            cfg.From(typeof(PersonalInfo), typeof(Parents));
            cfg.ApplyingConvention(new StaticValueResolver(systemId), m => m.SystemId);
            cfg.UsingValue<PersonalInfo>(myManualSystemId, on => on.SystemId);
            var bindable = bindableFactory.CreateBindableConfiguration(cfg.ToSnapshot());
//            bindable.Bind();
//            Action validation = bindable.Assert;
//            validation.should_not_throw_an<MappingConfigurationException>();
            var executable = bindable.CreateExecutableMapping(typeof(PersonalInfo));

            executable.Execute(contextualizer.CreateContext(src1, destination));
            destination.SystemId.should_be_equal_to(myManualSystemId);
        }
        [Fact]
        public void it_should_support_other_property_specifications()
        {
            var cfg = new DestinationConfiguration(typeof(SystemPerson), new TestConfigurationFactory());
            cfg.From(typeof(PersonalInfo), typeof(Parents))
                .ApplyingConvention(new PrefixPropertyCriterion("System"), new IgnoreResolver());
            var bindable = bindableFactory.CreateBindableConfiguration(cfg.ToSnapshot());
            Action validation = bindable.Assert;
            validation.should_not_throw_an<MappingConfigurationException>();
        }
        [Fact]
        public void it_should_apply_conventions_to_types_of_properties()
        {
            var src1 = new PersonalInfo() { Age = 3, Name = "mikey" };
            var destination = new ManyDates();
            var dateTime = new DateTime(2009, 8, 1, 0, 0, 0);
            var cfg = new DestinationConfiguration(typeof(ManyDates), new TestConfigurationFactory());
            cfg.From(typeof (PersonalInfo), typeof (Parents))
                .ApplyingConvention(new TypePropertyCriterion(typeof (DateTime)), new StaticValueResolver(dateTime));
            var bindable = bindableFactory.CreateBindableConfiguration(cfg.ToSnapshot());
            
            var executable = bindable.CreateExecutableMapping(typeof (PersonalInfo));
            executable.Execute(contextualizer.CreateContext(src1, destination));
            destination.DateTime1.should_be_equal_to(dateTime);
            destination.DateTime2.should_be_equal_to(dateTime);
            destination.Name.should_be_equal_to("mikey");
        }
        
        public class ManyDates
        {
            public DateTime DateTime1 { get; set; }
            public DateTime DateTime2 { get; set; }
            public string Name { get; set; }
        }
        
    }
}