using System;

namespace Ditto.Internal
{
    public class SourceContextDiscovery
    {
        public SourceContextDiscovery(Type requestedType)
        {
            RequestedType = requestedType;
        }

        public Type TypeToUse { get; private set; }
        public Type RequestedType { get; private set; }
        public bool IsSatisfiedBy(SourceContext context)
        {
            if(RequestedType==context.SourceType)
            {
                TypeToUse = context.SourceType;
                return true;
            }

            var discovered = RequestedType.BaseType;
            while (discovered != null)
            {
                if(discovered==context.SourceType)
                {
                    TypeToUse = discovered;
                    return true;
                }
                discovered = discovered.BaseType;
            }

            if (RequestedType.GetInterfaces().Length <= 0)
            {
                return false;
            }
            foreach (var theInterfaceType in RequestedType.GetInterfaces())
            {
                if (context.SourceType == theInterfaceType)
                {
                    TypeToUse = theInterfaceType;
                    return true;
                }
            }
            return false;
        }
    }
}