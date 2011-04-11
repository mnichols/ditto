using System;
using System.Collections.Generic;
using System.Linq;

namespace Ditto.Internal
{
    public class ResolverPrecedence
    {
        public static IDictionary<Type,PrioritizedComposedFirstMatchingResolverContainer> ToResolverContainer(BindableConfiguration bindableConfiguration)
        {
            var dic = new Dictionary<Type, PrioritizedComposedFirstMatchingResolverContainer>();
            foreach (var sourceContext in bindableConfiguration.SourceContexts)
            {
                var copy = sourceContext;
                var sourcedConventions = new PrioritizedComposedFirstMatchingResolverContainer(
                    bindableConfiguration.SourcedConventions.Where(its => its.SourceType == copy.SourceType).ToArray());

                dic[sourceContext.SourceType]=new PrioritizedComposedFirstMatchingResolverContainer(new IContainResolvers[]{copy, sourcedConventions});

            }
            return dic;
        }
    }
}