using System;
using System.Collections;
using System.Linq;
using Castle.Core;
using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Ditto.Internal;

namespace Ditto.WindsorIntegration
{
    public class DittoFacility : AbstractFacility
    {
        private const string GlobalConventionsKey = "global.conventions";
        private const string DefaultMapCommandKey = "default.map.command";

        public void AssertFacility<T>() where T : IFacility, new()
        {
            if (Kernel.GetFacilities().Any(fac => fac.GetType() == typeof (T)))
                return;
            Kernel.AddFacility<T>();
        }

        private void AssertFacility<T>(Func<IFacility> createFacility) where T : IFacility
        {
            if (Kernel.GetFacilities().Any(fac => fac.GetType() == typeof (T)))
                return;
            Kernel.AddFacility(typeof (T).Name, createFacility());
        }

        protected override void Init()
        {
            AssertFacility<TypedFactoryFacility>();
            AssertFacility<LoggingFacility>(() => new LoggingFacility(LoggerImplementation.Log4net, "log4net.config"));
            RegisterInternalServices();
            RegisterNatives();
            RegisterApi();
        }

        private void RegisterApi()
        {
            Kernel.Register(Component.For<IContainDestinationConfiguration,IProvideDestinationConfigurationSnapshots>()
                                .ImplementedBy<DestinationConfigurationContainer>()
                                .ServiceOverrides(new {conventions = GlobalConventionsKey})
                                .LifeStyle.Singleton);
            Kernel.Register(Component.For<IMap>().ImplementedBy<DittoDoer>().LifeStyle.Singleton);
            Kernel.Register(Component.For<IInitializeDitto>().ImplementedBy<WindsorMappingInitializer>().LifeStyle.Transient);
            Kernel.Register(Component.For<IBindConfigurations, ICreateMappingCommand>().ImplementedBy<BindingDestinationConfigurationContainer>().LifeStyle.Singleton);
        }

        private void RegisterInternalServices()
        {
//            Kernel.Register(Component.For<IActivator>().ImplementedBy<NativeActivator>().LifeStyle.Transient);
            Kernel.Register(
                Component.For<ICreateValueAssignment>().ImplementedBy<ValueAssignments>().LifeStyle.Transient);
            Kernel.Register(
                Component.For<IContextualizeResolution>().ImplementedBy<DefaultContextualizer>().LifeStyle.Singleton);
            Kernel.Register(
                Component.For<IInvoke, ICacheInvocation,IActivate>().ImplementedBy<Fasterflection>().LifeStyle.Transient);
            Kernel.Register(Component.For<IVisitCacheable>().ImplementedBy<CacheInitializer>().LifeStyle.Transient);
            Kernel.Register(Component.For<IValueConverterContainer>().ImplementedBy<DefaultValueConverterContainer>().LifeStyle.
                    Singleton);
            Kernel.Resolver.AddSubResolver(new MapCommandResolver(Kernel));
            Kernel.Register(Component.For<IMapCommand>()
                                .ImplementedBy<CloningMapCommand>()
                                .ServiceOverrides(new {inner = DefaultMapCommandKey})
                                .LifeStyle.Transient);

            Kernel.Register(
                Component.For<IMapCommand>().ImplementedBy<DefaultMapCommand>().Named(DefaultMapCommandKey).LifeStyle.
                    Transient);
            Kernel.Register(
                Component.For<IContainGlobalConventions>().ImplementedBy<GlobalConventions>().Named(
                    GlobalConventionsKey).LifeStyle.Singleton);
            Kernel.Register(Component.For<IMapCommandFactory>().AsFactory()); /*typedfactory*/
            Kernel.Register(Component.For<ICreateDestinationConfiguration>().AsFactory()); /*typedfactory*/
            Kernel.Register(Component.For<ICreateBindableConfiguration>().AsFactory()); /*typedfactory*/
            Kernel.Register(Component.For<BindableConfiguration>().LifeStyle.Transient);
            Kernel.Register(
                Component.For<IConfigureDestination, ISourcedDestinationConfiguration>().ImplementedBy
                    <DestinationConfiguration>().LifeStyle.Transient);
            Kernel.Register(
                Component.For(typeof (IConfigureDestination<>), typeof (ISourcedDestinationConfiguration<>)).
                    ImplementedBy(typeof (DestinationConfiguration<>)).LifeStyle.Transient);
            Kernel.Register(Component.For<IProvideBinders>().ImplementedBy<BinderFactory>().LifeStyle.Singleton);
            Kernel.Register(Component.For<ILogFactory>().ImplementedBy<WindsorLoggerFactory>().LifeStyle.Singleton);
        }

        private void RegisterNatives()
        {
            Kernel.Register(
                Component.For<IConfigureGlobalConventions>().ImplementedBy<NativeConverters>().LifeStyle.Singleton);
        }

        /// <summary>
        ///   This resolver makes up for the TypedFactoryFacility failure in forwarding parameters into child components for IMapCommand
        /// </summary>
        public class MapCommandResolver : ISubDependencyResolver
        {
            private readonly IKernel kernel;

            public MapCommandResolver(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
                                  ComponentModel model, DependencyModel dependency)
            {
                object inner = null;
                try
                {
                    var executableMapping = context.AdditionalParameters["executableMapping"];
                    if (executableMapping == null)
                        throw new InvalidOperationException(
                            "Expected to resolve with a ctor parameter of 'executableMapping'");
                    inner = kernel.Resolve<IMapCommand>(DefaultMapCommandKey,
                                                        new Hashtable {{"executableMapping", executableMapping}});
                    return inner;
                }
                finally
                {
                    kernel.ReleaseComponent(inner);
                }
            }

            public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
                                   ComponentModel model, DependencyModel dependency)
            {
                return model.Service == typeof (IMapCommand) && model.Name != DefaultMapCommandKey &&
                       dependency.TargetItemType == typeof (IMapCommand);
            }
        }
    }
}