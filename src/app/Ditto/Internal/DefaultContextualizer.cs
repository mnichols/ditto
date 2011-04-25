using System;

namespace Ditto.Internal
{
    public class DefaultContextualizer:IContextualizeResolution
    {
        private readonly IActivate activate;
        private readonly ICreateValueAssignment valueAssignments;
        private readonly IInvoke invoke;

        public DefaultContextualizer(IActivate activate, ICreateValueAssignment valueAssignments, IInvoke invoke)
        {
            this.activate = activate;
            this.valueAssignments = valueAssignments;
            this.invoke = invoke;
        }

        public IResolutionContext CreateContext(object source, object destination)
        {
            return new DefaultResolutionContext(source, destination, activate, this, valueAssignments,invoke);
        }

        public IResolutionContext CreateContext(object source, Type destinationType)
        {
            var destination = activate.CreateInstance(destinationType);
            return new DefaultResolutionContext(source, destination, activate, this, valueAssignments, invoke);
        }

        public IResolutionContext CreateContext(IDescribeMappableProperty sourceProperty)
        {
            return new NullSourceContext(sourceProperty, valueAssignments, invoke,activate);
        }
    }
}