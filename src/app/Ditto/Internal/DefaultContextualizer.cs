using System;

namespace Ditto.Internal
{
    public class DefaultContextualizer:IContextualizeResolution
    {
        private readonly IActivator activator;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultContextualizer(IActivator activator, ICreateValueAssignment valueAssignments, IInvoke invoke)
        {
            this.activator = activator;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext CreateContext(object source, object destination)
        {
            return new DefaultResolutionContext(source, destination, activator, this, valueAssignments,invoke);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            return new DefaultResolutionContext(source, activator.CreateInstance(destinationType), activator, this, valueAssignments, invoke);
        }
    }
}