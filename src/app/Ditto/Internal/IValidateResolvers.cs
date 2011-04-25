using System;

namespace Ditto.Internal
{
    public interface  IValidateResolvers
    {
        /// <summary>
        /// Determines whether this container has resolvers from a source <i>other</i> than the destination type.
        /// </summary>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="destinationProperty">The destination property.</param>
        /// <returns>
        /// 	<c>true</c> if it has resolver from other source type than the destination type; otherwise, <c>false</c>.
        /// </returns>
        bool HasResolverFromOtherSource(Type destinationType, IDescribeMappableProperty destinationProperty);
    }
}