using Ditto.Internal;

namespace Ditto.Resolvers
{
    /// <summary>
    /// Always resolve using the provided <para>value</para>.
    /// </summary>
    public class StaticValueResolver:IResolveValue
    {
        public object Value { get;private set; }

        public StaticValueResolver(object value)
        {
            Value = value;
        }
        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return new Result(true, Value);
        }

    }
}