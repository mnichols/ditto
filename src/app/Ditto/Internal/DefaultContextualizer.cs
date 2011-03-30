using System;

namespace Ditto.Internal
{
    public class DefaultContextualizer:IContextualizeResolution
    {
        private IActivator activator;
        private ICreateValueAssignment valueAssignments;
        private IReflection reflection;

        public DefaultContextualizer(IActivator activator, ICreateValueAssignment valueAssignments, IReflection reflection)
        {
            this.activator = activator;
            this.valueAssignments = valueAssignments;
            this.reflection = reflection;
        }

        public IResolutionContext CreateContext(object source, object destination)
        {
            return new DefaultResolutionContext(source, destination, activator, this, valueAssignments,reflection);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            return new DefaultResolutionContext(source, activator.CreateInstance(destinationType), activator, this, valueAssignments, reflection);
        }
    }
}