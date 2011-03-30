using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class StaticValueResolver:IResolveValue
    {
        public object Value { get;private set; }

        public StaticValueResolver(object value)
        {
            Value = value;
        }
        public Result TryResolve(IResolutionContext context)
        {
            return new Result(true, Value);
        }

    }
}