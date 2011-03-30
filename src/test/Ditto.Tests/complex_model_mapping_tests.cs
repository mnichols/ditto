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

        public complex_model_mapping_tests()
        {
            contextualizer = new TestContextualizer();
        }
        [Fact]
        public void it_should_map_nested_components()
        {
            var componentConfig = new DestinationConfiguration(typeof(ViewModelComponent));
            componentConfig.From(typeof(EventComponent));
            componentConfig.Bind();
            componentConfig.Assert();

            var modelConfig = new DestinationConfiguration(typeof(ComplexViewModel));
            modelConfig.From(typeof (ComplexEvent));
            modelConfig.Bind(componentConfig);
            modelConfig.Assert();

            var source = new ComplexEvent(){Name = "RootName", Component = new EventComponent() {Name = "ComponentName"}};
            var dest = new ComplexViewModel();
            var executable = modelConfig.CreateExecutableMapping(typeof (ComplexEvent));
            executable.Execute(contextualizer.CreateContext(source, dest));
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");
        }
        [Fact]
        public void it_should_map_nested_components_by_type()
        {
            var componentConfig = new DestinationConfiguration(typeof(ViewModelComponent));
            componentConfig.From(typeof(EventComponent));
            componentConfig.Bind();
            componentConfig.Assert();

            var modelConfig = new DestinationConfiguration(typeof(ComplexViewModel));
            modelConfig.From(typeof (ComplexEventWithDifferentNamedComponent));
            modelConfig.SetPropertyResolver(
                PropertyNameCriterion.From<ComplexViewModel>(m=>m.Component), typeof(ComplexEventWithDifferentNamedComponent),
                new RedirectingConfigurationResolver(MappableProperty.For<ComplexEventWithDifferentNamedComponent>(s => s.DifferentName), MappableProperty.For<ComplexViewModel>(m => m.Component), componentConfig));
            modelConfig.Bind();
            modelConfig.Assert();

            var source = new ComplexEventWithDifferentNamedComponent() { Name = "RootName", DifferentName= new EventComponent() { Name = "ComponentName" } };
            var dest = new ComplexViewModel();
            var executable = modelConfig.CreateExecutableMapping(typeof(ComplexEventWithDifferentNamedComponent));
            executable.Execute(contextualizer.CreateContext(source, dest));
            dest.Name.should_be_equal_to("RootName");
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("ComponentName");
        }
        [Fact]
        public void nested_configurations_are_validated()
        {
            var modelConfig = new DestinationConfiguration<DestinationWrapper>(new TestDestinationConfigurationFactory());
            modelConfig.From<SourceWithIncompleteMembers>();
            modelConfig.Nesting<SourceWithIncompleteMembers, DestinationWithUnmappedMember>(its => its.Component, cfg =>{});
            modelConfig.Bind();
            Action validation=modelConfig.Assert;
            validation.should_throw_because<MappingConfigurationException>("The following properties are not mapped for '" + typeof(DestinationWithUnmappedMember) + "':" + Environment.NewLine + "UnmappedMember" + Environment.NewLine);
        }
        [Fact]
        public void it_should_redirect_nested_mappings_to_dest_property()
        {

            var modelConfig = new DestinationConfiguration<ComplexViewModel>(new TestDestinationConfigurationFactory());
            modelConfig.From<ComplexEvent,TypicalEvent>();
            modelConfig.Nesting<TypicalEvent,ViewModelComponent>(its=>its.Component,cfg => { });
            modelConfig.Bind();
            modelConfig.Assert();

            var source = new TypicalEvent() { Name = "RedirectingName",};
            var dest = new ComplexViewModel();
            var executable = modelConfig.CreateExecutableMapping(typeof(TypicalEvent));
            executable.Execute(contextualizer.CreateContext(source, dest));
            dest.Component.should_not_be_null();
            dest.Component.Name.should_be_equal_to("RedirectingName");
        }
        
        
        [Fact]
        public void it_should_map_nested_models()
        {
            var int1 = new DestinationConfiguration(typeof (NestedPropsDestination.Int1));
            int1.From(typeof (NestedPropsSource.Int1));
            int1.Bind();
            var int2 = new DestinationConfiguration(typeof (NestedPropsDestination.Int2));
            int2.From(typeof (NestedPropsSource.Int2));
            int2.Bind(int1);
            var cfg = new DestinationConfiguration(typeof (NestedPropsDestination));
            cfg.From(typeof (NestedPropsSource));
            cfg.Bind(int2);
            cfg.Assert();
            var executable = cfg.CreateExecutableMapping(typeof (NestedPropsSource));
            var mapper = new DefaultMapCommand(executable, contextualizer);
            var src = new NestedPropsSource();
            var dest = new NestedPropsDestination();
            mapper.Map(src, dest);
        }
    }
}