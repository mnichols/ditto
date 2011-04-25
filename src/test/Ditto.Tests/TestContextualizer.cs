using System;
using Ditto.Internal;

namespace Ditto.Tests
{
    public class TestContextualizer : IContextualizeResolution,IMapCommandFactory,ICacheInvocation
    {
        private Fasterflection reflection;
        private IContextualizeResolution inner;
        public TestContextualizer()
        {
            reflection = new Fasterflection();
            inner = new DefaultContextualizer(reflection,
                                              new ValueAssignments(new DefaultValueConverterContainer(reflection),
                                                                   reflection), reflection);
        }
        public IResolutionContext CreateContext(object source, object destination)
        {
            return inner.CreateContext(source, destination);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            return inner.CreateContext(source, destinationType);
        }

        public IResolutionContext CreateNestedParallelContext(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty, IResolutionContext parentContext)
        {
            return inner.CreateNestedParallelContext(sourceProperty, destinationProperty, parentContext);
        }

        public IResolutionContext CreateNestedContext(IDescribeMappableProperty destinationProperty, IResolutionContext parentContext)
        {
            return inner.CreateNestedContext(destinationProperty, parentContext);
        }

        public IMapCommand Create(IExecuteMapping executableMapping)
        {
            return new DefaultMapCommand(executableMapping, this);
        }

        public GetValue CacheGet(Type targetType, string propertyName)
        {
            return reflection.CacheGet(targetType, propertyName);
        }

        public SetValue CacheSet(Type targetType, string propertyName)
        {
            return reflection.CacheSet(targetType, propertyName);
        }
    }
}