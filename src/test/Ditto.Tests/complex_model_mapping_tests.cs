using System;
using Ditto.Criteria;
using Ditto.Internal;
using Ditto.Resolvers;
using Xunit;

namespace Ditto.Tests
{
    public class complex_model_mapping_tests
    {
        private TestContextualizer contextualizer;
        private DestinationConfigurationContainer container;
        private TestConfigurationFactory configFactory;
        public complex_model_mapping_tests()
        {
            contextualizer = new TestContextualizer();
            configFactory=new TestConfigurationFactory();
            container = new DestinationConfigurationContainer(null, configFactory);
        }
        [Fact]
        public void it_should_map_nested_components()
        {
            container.Map(typeof (ViewModelComponent)).From(typeof (EventComponent));
            container.Map(typeof (ComplexViewModel)).From(typeof (ComplexEvent));
            var bindable = container.ToBinding();
            bindable.Bind();
            bindable.Assert();

            var source = new ComplexEvent(){Name = "RootName", Component = new EventComponent() {Name = "ComponentName"}};
            var dest = new ComplexViewModel();
            var executable = bindable.CreateCommand(typeof (ComplexEvent), typeof (ComplexViewModel));
            executable.Map(source, dest);
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");
        }
        [Fact]
        public void it_should_map_nested_components_by_type()
        {
            var componentConfig = new DestinationConfiguration(typeof(ViewModelComponent));
            componentConfig.From(typeof(EventComponent));


            var modelConfig = new DestinationConfiguration(typeof(ComplexViewModel));
            modelConfig.From(typeof (ComplexEventWithDifferentNamedComponent));
            modelConfig.SetPropertyResolver(
                PropertyNameCriterion.From<ComplexViewModel>(m=>m.Component), typeof(ComplexEventWithDifferentNamedComponent),
                new RedirectingConfigurationResolver(MappableProperty.For<ComplexEventWithDifferentNamedComponent>(s => s.DifferentName), configFactory.CreateBindableConfiguration(componentConfig.TakeSnapshot())));

            var bindable = configFactory.CreateBindableConfiguration(modelConfig.TakeSnapshot());
          

            var source = new ComplexEventWithDifferentNamedComponent() { Name = "RootName", DifferentName= new EventComponent() { Name = "ComponentName" } };
            var dest = new ComplexViewModel();
            var executable = bindable.CreateExecutableMapping(typeof(ComplexEventWithDifferentNamedComponent));
            executable.Execute(contextualizer.CreateContext(source, dest));
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");
        }
        [Fact]
        public void it_should_map_nested_components_by_type_with_generic_sugar()
        {
            var container = new DestinationConfigurationContainer(null, configFactory);
            container.Map<ComplexViewModel>()
                .From<ComplexEventWithDifferentNamedComponent>()
                .Redirecting<ComplexEventWithDifferentNamedComponent,ViewModelComponent>(from => from.DifferentName, to => to.Component,nested=>{});
            var binding = container.ToBinding();
            binding.Bind();
            binding.Assert();
            var source = new ComplexEventWithDifferentNamedComponent() { Name = "RootName", DifferentName = new EventComponent() { Name = "ComponentName" } };
            var dest = new ComplexViewModel();
            var cmd=binding.CreateCommand(source.GetType(), dest.GetType());
            cmd.Map(source, dest);
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");
        }
        [Fact]
        public void nested_configurations_are_validated()
        {
            container.Map<DestinationWrapper>()
                .From<SourceWithIncompleteMembers>()
                .Nesting<SourceWithIncompleteMembers, DestinationWithUnmappedMember>(its => its.Component, cfg => { });
            var bindable = container.ToBinding();
            bindable.Bind();
            
            Action validation=bindable.Assert;
            validation.should_throw_because<DittoConfigurationException>("The following properties are not mapped for '" + typeof(DestinationWithUnmappedMember) + "':" + Environment.NewLine + "UnmappedMember" + Environment.NewLine);
        }
        [Fact]
        public void it_should_redirect_nested_mappings_to_dest_property()
        {
            container.Map<ComplexViewModel>()
                .From<ComplexEvent,TypicalEvent>()
                .Nesting<TypicalEvent,ViewModelComponent>(its=>its.Component,cfg => { });
            var bindable = container.ToBinding();
            bindable.Bind();
            bindable.Assert();

            var source = new TypicalEvent() { Name = "RedirectingName",};
            var dest = new ComplexViewModel();
            var executable = bindable.CreateCommand(typeof (TypicalEvent), typeof (ComplexViewModel));
            executable.Map(source, dest);
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("RedirectingName");
        }
        
        [Fact]
        public void it_should_map_nested_models()
        {
            container.Map(typeof (NestedPropsDestination.Int1)).From(typeof (NestedPropsSource.Int1));
            container.Map(typeof (NestedPropsDestination.Int2)).From(typeof (NestedPropsSource.Int2));
            container.Map(typeof (NestedPropsDestination)).From(typeof (NestedPropsSource));
            var bindable = container.ToBinding();
            bindable.Bind();
            bindable.Assert();

            var mapper = bindable.CreateCommand(typeof(NestedPropsSource), typeof(NestedPropsDestination));
            var src = new NestedPropsSource();
            var dest = new NestedPropsDestination();
            mapper.Map(src, dest);
        }
    }
}