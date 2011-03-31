using System;
using Ditto.Internal;

namespace Ditto.Benchmarking.Tests
{
    public class TestContextualizer : IContextualizeResolution,ICacheInvocation
    {
        private Fasterflection reflection;

        public TestContextualizer()
        {
            reflection = new Fasterflection();
        }
        public IResolutionContext CreateContext(object source, object destination)
        {

            return new DefaultResolutionContext(source, destination, new Fasterflection(), this,
                                                new ValueAssignments(new DefaultValueConverterContainer(reflection),
                                                                     reflection), reflection);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            return new DefaultResolutionContext(source, reflection.CreateInstance(destinationType), reflection, this,
                                                new ValueAssignments(new DefaultValueConverterContainer(reflection), reflection), reflection);
        }

        public GetValue CacheGet(Type targetType, string propertyName)
        {
            return reflection.CacheGet(targetType,propertyName);
        }

        public SetValue CacheSet(Type targetType, string propertyName)
        {
            return reflection.CacheSet(targetType,propertyName);
        }
    }
}