using System;
using Ditto.Criteria;
using Ditto.Internal;
using Ditto.Resolvers;
using Xunit;

namespace Ditto.Tests
{
    public class mapping_configuration_tests
    {
        
        [Fact]
        public void missing_property_mapping_throws()
        {
            var cfg = new DestinationConfiguration(typeof(Person), new TestDestinationConfigurationFactory());
            cfg.From(typeof (PersonalInfo), typeof (Parents));
            var bindable = cfg.CreateBindableConfiguration();
            Action validation = bindable.Assert;
            validation.should_throw_an<MappingConfigurationException>();
            validation.should_throw_because<MappingConfigurationException>("The following properties are not mapped for '" + typeof(Person) + "':" + Environment.NewLine + "FathersName" + Environment.NewLine);
        }

        [Fact]
        public void can_map_from_property_of_diff_name()
        {
            var cfg = new DestinationConfiguration<TypicalViewModel>(new TestDestinationConfigurationFactory());
            cfg.From(typeof (TypicalEvent)).Redirecting<TypicalEvent>(its => its.Id, its => its.SomeId);
            var bindable = cfg.CreateBindableConfiguration();
            Action validation = bindable.Assert;
            validation.should_not_throw_an<MappingConfigurationException>();
        }

        [Fact]
        public void can_map_static_value_to_property()
        {
            var cfg = new DestinationConfiguration<TypicalViewModel>(new TestDestinationConfigurationFactory());
            cfg.From(typeof (TypicalEvent)).UsingValue<TypicalEvent>(Guid.NewGuid(), its => its.SomeId);
            var bindable = cfg.CreateBindableConfiguration();
            Action validation = bindable.Assert;
            validation.should_not_throw_an<MappingConfigurationException>();
        }
        [Fact]
        public void can_append_other_configuration_for_nested_props()
        {
            var componentConfig = new DestinationConfiguration(typeof(ViewModelComponent), new TestDestinationConfigurationFactory());
            componentConfig.From(typeof (EventComponent));


            var modelConfig = new DestinationConfiguration(typeof(ComplexViewModel), new TestDestinationConfigurationFactory());
            modelConfig.From(typeof (ComplexEvent));

            var bindable = modelConfig.CreateBindableConfiguration();
//            bindable.Bind(componentConfig.CreateBindableConfiguration());
            bindable.Assert();

        }

        [Fact]
        public void configurations_can_be_bound_to_one_another()
        {
            var cfg = new DestinationConfigurationContainer(null, new TestDestinationConfigurationFactory());
            cfg.Map(typeof(ViewModelComponent)).From(typeof(EventComponent));
            cfg.Map(typeof (ComplexViewModel)).From(typeof (ComplexEvent));
            var bindable = cfg.ToBindable();
            bindable.Bind();
            bindable.Assert();

            var source = new ComplexEvent() { Name = "RootName", Component = new EventComponent() { Name = "ComponentName" } };
            var dest = new ComplexViewModel();
            var command = bindable.CreateCommand(typeof (ComplexEvent), typeof (ComplexViewModel));
            command.Map(source, dest);
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");

        }
        [Fact]
        public void configurations_can_be_bound_to_one_another_using_binding_container()
        {
            var cfg = new DestinationConfigurationContainer(null, new TestDestinationConfigurationFactory());
            cfg.Map(typeof(ViewModelComponent)).From(typeof(EventComponent));
            cfg.Map(typeof(ComplexViewModel)).From(typeof(ComplexEvent));
            var bindable = cfg.ToBindable();
            bindable.Bind();
            bindable.Assert();
            
            var source = new ComplexEvent() { Name = "RootName", Component = new EventComponent() { Name = "ComponentName" } };
            var dest = new ComplexViewModel();
            var command = bindable.CreateCommand(typeof(ComplexEvent), typeof(ComplexViewModel));
            command.Map(source, dest);
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");

        }

        [Fact]
        public void manually_configuring_dest_property_with_more_than_one_resolver()
        {
            var cfg = new DestinationConfiguration(typeof(TypicalViewModel), new TestDestinationConfigurationFactory());
            cfg.From(typeof (TypicalEvent));
            cfg.SetPropertyResolver(new PropertyNameCriterion("SomeId"),typeof(TypicalEvent),new StaticValueResolver(new Guid("8CF7C50E-792D-4A28-AB74-81879BC233A8")));
            Action duplication=()=>cfg.SetPropertyResolver(new PropertyNameCriterion("SomeId"), typeof(TypicalEvent), new StaticValueResolver(new Guid("1B8CF33D-92B8-4E82-9E8F-5EEDE7BA14F0")));
            duplication.should_throw_because<MappingConfigurationException>("Destination property 'Ditto.Tests.TypicalViewModel:SomeId' already has a manually, or conventioned, configured resolver from source type 'Ditto.Tests.TypicalEvent'");

        }
        [Fact]
        public void manually_configuring_dest_property_is_not_overriden_by_convention()
        {
            var cfg = new DestinationConfiguration(typeof(TypicalViewModel), new TestDestinationConfigurationFactory());
            cfg.From(typeof(TypicalEvent));
            cfg.SetPropertyResolver(new PropertyNameCriterion("SomeId"), typeof(TypicalEvent), new StaticValueResolver(new Guid("8CF7C50E-792D-4A28-AB74-81879BC233A8")));
            cfg.ApplyingConvention(new PropertyNameCriterion("SomeId"), new StaticValueResolver(new Guid("1B8CF33D-92B8-4E82-9E8F-5EEDE7BA14F0")));
            Action duplication = () => cfg.CreateBindableConfiguration().Bind();
            duplication.should_not_throw_an<MappingConfigurationException>();
        }
    }

}