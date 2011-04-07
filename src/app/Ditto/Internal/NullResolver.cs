namespace Ditto.Internal
{
    public class NullResolver:IResolveValue
    {
        private static readonly IResolveValue instance;
        public static IResolveValue Instance { get { return instance; } }
        static NullResolver()
        {
            instance = new NullResolver();
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            return Result.Unresolved;
        }
    }
}