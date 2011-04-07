using System;
using Ditto.Internal;

namespace Ditto.Tests
{
    public class TestContextualizer : IContextualizeResolution,IMapCommandFactory,ICacheInvocation
    {
        private Fasterflection reflection;

        public TestContextualizer()
        {
            reflection = new Fasterflection();
        }
        public IResolutionContext CreateContext(object source, object destination)
        {

            return new DefaultResolutionContext(source, destination, reflection, this,
                                                new ValueAssignments(new DefaultValueConverterContainer(reflection),
                                                                     reflection),reflection);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            return new DefaultResolutionContext(source, reflection.CreateInstance(destinationType), reflection, this,
                                                new ValueAssignments(new DefaultValueConverterContainer(reflection),reflection),reflection);
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