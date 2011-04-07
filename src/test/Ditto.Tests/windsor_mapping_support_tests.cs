using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Ditto.Converters;
using Ditto.Criteria;
using Ditto.Resolvers;
using Ditto.WindsorIntegration;
using Xunit;

namespace Ditto.Tests
{
    public class windsor_mapping_support_tests : IDisposable
    {
        private WindsorContainer container;

        public windsor_mapping_support_tests()
        {
            container = new WindsorContainer(new XmlInterpreter());
            
        }
        
        private void WithConfiguration<T>() where T : IConfigureMapping
        {
            container.AddFacility("mapping", new MappingFacility());
            container.Register(Component.For<IConfigureMapping>().ImplementedBy<T>().LifeStyle.Transient);
        }
        
        [Fact]
        public void configuration_registration_of_mappings_is_okay()
        {
            container.AddFacility("mapping", new MappingFacility());
            container.RegisterAllConfigurationsIn(new[] { GetType().Assembly });
            var configurations = container.Kernel.GetHandlers(typeof (IConfigureMapping));
            configurations.Length.should_not_be_equal_to(0);
            configurations.Any(it => it.ComponentModel.Implementation == typeof (PeopleConfiguration)).should_be_true();
        }
        [Fact]
        public void configuration_execution_works()
        {
            container.AddFacility("mapping", new MappingFacility());
            container.RegisterAllConfigurationsIn(new[] {GetType().Assembly});
            var configurations=container.ResolveAll<IConfigureMapping>();
            foreach (var configureMapping in configurations)
            {
                configureMapping.Configure();
            }
            var configs = container.Resolve<IContainDestinationConfiguration>();
            configs.HasConfigurationFor(typeof (Person)).should_be_true();

        }
        [Fact]
        public void mapping_execution_works()
        {
            WithConfiguration<PeopleConfiguration>();
            var configurations = container.Resolve<IInitializeMappingEngine>();
            configurations.Initialize();
            
            var mapper = container.Resolve<IMap>();

            var output = mapper.Map<Person>(new PersonalInfo() { Age = 3, Name = "mikey" });
            output.Age.should_be_equal_to(3);
            output.Name.should_be_equal_to("mikey");
            output.MothersName.should_be_equal_to(null);
            container.Release(mapper);
            container.Release(configurations);
        }
        [Fact]
        public void global_converters_are_applied()
        {
            WithConfiguration<PeopleConfiguration>();
            var configurations = container.Resolve<IInitializeMappingEngine>();
            var testGlobals = new TestGlobals(container,new List<IConvertValue>{new DateTimeUtcConverter()},null);
            container.Register(Component.For<IConfigureGlobalConventions>().Instance(testGlobals));
            configurations.Initialize();

            var mapper = container.Resolve<IMap>();

            var notUtc=new DateTime(2004,12,7,5,4,3);
            var output = mapper.Map<Person>(new PersonalInfo() { Age = 3, Name = "mikey" ,Birthdate = notUtc});
            output.Age.should_be_equal_to(3);
            output.Name.should_be_equal_to("mikey");
            output.MothersName.should_be_equal_to(null);
            output.Birthdate.should_be_equal_to(notUtc.ToUniversalTime());
            output.Birthdate.Kind.should_be_equal_to(DateTimeKind.Utc);
            container.Release(mapper);
            container.Release(configurations);

        }
        [Fact]
        public void global_ignore_resolvers_are_applied()
        {
            WithConfiguration<SystemPersonConfiguration>();
            var configurations = container.Resolve<IInitializeMappingEngine>();
            var testGlobals = new TestGlobals(container,null, new Dictionary<IPropertyCriterion, IResolveValue>
                                                 {
                                                     {PropertyNameCriterion.From<SystemPerson>(p=>p.SystemId), new IgnoreResolver()}
                                                 });
            container.Register(Component.For<IConfigureGlobalConventions>().Instance(testGlobals));
            configurations.Initialize();

            var mapper = container.Resolve<IMap>();

            var output = mapper.Map<SystemPerson>(new PersonalInfo() { Age = 3, Name = "mikey",});
            output.Age.should_be_equal_to(3);
            output.Name.should_be_equal_to("mikey");
            output.Age.should_be_equal_to(3);
            output.SystemId.should_be_equal_to(Guid.Empty);
            container.Release(mapper);
            container.Release(configurations);

        }
        [Fact]
        public void global_behavioral_resolvers_are_applied()
        {
            WithConfiguration<SystemPersonConfiguration>();
            
            var configurations = container.Resolve<IInitializeMappingEngine>();
            Guid id = Guid.Empty;
            var testGlobals = new TestGlobals(container, null, new Dictionary<IPropertyCriterion, IResolveValue>
                                                 {
                                                     {PropertyNameCriterion.From<SystemPerson>(p=>p.SystemId), new LambdaResolver<SystemPerson>(o=>id=Guid.NewGuid())},
                                                     {new TypePropertyCriterion(typeof(int)),new LambdaResolver<PersonalInfo>(val=>val.Age+2)}
                                                 });
            container.Register(Component.For<IConfigureGlobalConventions>().Instance(testGlobals));
            configurations.Initialize();

            var mapper = container.Resolve<IMap>();

            var output = mapper.Map<SystemPerson>(new PersonalInfo() { Age = 3, Name = "mikey", });
            
            output.Name.should_be_equal_to("mikey");
            output.SystemId.should_be_equal_to(id);
            output.Age.should_be_equal_to(5);
            container.Release(mapper);
            container.Release(configurations);

        }


        public class TestGlobals:AbstractGlobalConventionConfiguration
        {
            private readonly IWindsorContainer container;
            List<IConvertValue> converters;
            Dictionary<IPropertyCriterion,IResolveValue> resolvers;

            public TestGlobals()
            {
                
            }
            public TestGlobals(
                IWindsorContainer   container,
                List<IConvertValue> converters, Dictionary<IPropertyCriterion, IResolveValue> resolvers)
            {
                this.container = container;
                this.converters = converters??new List<IConvertValue>();
                this.resolvers = resolvers??new Dictionary<IPropertyCriterion, IResolveValue>();

            }
            
            public override void Configure()
            {
                Conventions = container.Resolve<IGlobalConventionContainer>();
                foreach (var converter in converters)
                {
                    Conventions.AddConverter(converter);
                }
                foreach (var pair in resolvers)
                {
                    Conventions.AddResolver(pair.Key,pair.Value);
                }
            }
        }
        
        public class PeopleConfiguration:AbstractMappingConfiguration
        {
            public override void Configure()
            {
                Cfg.Map<Person>()
                    .From(typeof (PersonalInfo), typeof (Parents))
                    .ApplyingConvention(new IgnoreResolver(), it => it.FathersName);
            }
        }
        public class SystemPersonConfiguration : AbstractMappingConfiguration
        {
            public override void Configure()
            {
                Cfg.Map<SystemPerson>()
                    .From(typeof (PersonalInfo), typeof (Parents));
            }
        }

        /*
        private IInitializeMapCommand commandFactory;
        
        [Fact]
        public void it_should_map_using_provided_configuration()
        {
            commandFactory = Dependency<IInitializeMapCommand>();
            var cfg = new DefaultDestinationConfigurationContainer(commandFactory);
            commandFactory.Expect(x=>x.Get(null)).Return(new DefaultMapCommand(cfg.))
            cfg.Map<Person>()
                .From(typeof (PersonalInfo), typeof (Parents))
                .WithConvention(it => it.FathersName, new IgnoreResolver());
            var mapper = new DefaultMappingEngine(cfg);
            var output = mapper.Map<Person>(new PersonalInfo() {Age = 3, Name = "mikey"});
            output.Age.should_be_equal_to(3);
            output.Name.should_be_equal_to("mikey");
            output.MothersName.should_be_equal_to(null);
        }
         * */
        public void Dispose()
        {
            container.Dispose();
        }
    }
}