using Ditto.Internal;

namespace Ditto
{
    public interface IResolveValue
    {
        Result TryResolve(IResolutionContext context);
    }
}