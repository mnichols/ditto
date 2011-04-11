using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class ExecutableMapping : IExecuteMapping
    {
        private readonly IDescribeMappableProperty[] destinationProperties;
        private readonly IContainResolvers resolverContainer;
        private static Hashtable prop2Resolver=new Hashtable();
        public ExecutableMapping(Type destinationType, IContainResolvers resolverContainer,IDescribeMappableProperty[] destinationProperties)
        {
            DestinationType = destinationType;
            this.resolverContainer = resolverContainer;
            this.destinationProperties = destinationProperties;
        }

        public Type DestinationType { get; set; }


        public void Execute(IResolutionContext context)
        {
            foreach (var prop in destinationProperties.Where(p => resolverContainer.WillResolve(p)))
            {
                var assignment = context.BuildValueAssignment(prop);
                //TODO bottle neck to remove...resolver discovery
                var resolver = resolverContainer.GetResolver(prop);

                var result = resolver.TryResolve(context, prop);
                assignment.SetValue(result);
            }
        }
        private class DestProp2Source
        {
            private IDescribeMappableProperty destProp;
            private Type source;

            public DestProp2Source(IDescribeMappableProperty destProp, Type source)
            {
                this.destProp = destProp;
                this.source = source;
            }

            public bool Equals(DestProp2Source other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.destProp, destProp) && Equals(other.source, source);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (DestProp2Source)) return false;
                return Equals((DestProp2Source) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (destProp.GetHashCode()*397) ^ source.GetHashCode();
                }
            }
        }

    }
}