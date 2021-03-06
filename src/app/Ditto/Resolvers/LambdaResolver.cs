using System;
using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public class LambdaResolver<TSource>:IResolveValue
    {
        private Func<TSource, object> lambda;

        public LambdaResolver(Func<TSource, object> lambda)
        {
            this.lambda = lambda;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            if (typeof(TSource).IsAssignableFrom(context.SourceType) == false)
                return Result.Unresolved;

            return  new Result(true,lambda((TSource)context.Source)) ;
        }
    }
}