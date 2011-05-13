using System;
using System.Linq.Expressions;
using Ditto.Resolvers;

namespace Ditto
{
    public static class DittoExtensions
    {
        public static TDestination MultiMap<TDestination>(this IMap map,params object[] sources)
        {
            var dest=map.Map<TDestination>(sources[0]);
            if(sources.Length==1)
                return dest;
            for (int i = 1; i < sources.Length; i++)
            {
                map.Map(sources[i], dest);
            }
            return dest;
        }
        public static ISourcedDestinationConfiguration<T> Ignoring<T>(this ISourcedDestinationConfiguration<T> cfg, params Expression<Func<T, object>>[] properties)
        {
            return cfg.ApplyingConvention(new IgnoreResolver(), properties);
        }
        public static ISourcedDestinationConfiguration<T> AsImmutable<T>(this ISourcedDestinationConfiguration<T> cfg, params Expression<Func<T, object>>[] destinationProperties)
        {
            foreach (var destinationProperty in destinationProperties)
            {
                cfg.ApplyingConvention(new ImmutableDestinationResolver(new PropertyNameResolver(Reflect.GetProperty(destinationProperty).Name)), destinationProperty);
            }

            return cfg;
        }
    
    }
}