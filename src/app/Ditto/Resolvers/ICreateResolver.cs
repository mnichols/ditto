using Ditto.Internal;

namespace Ditto.Resolvers
{
    public interface ICreateResolver
    {
        IResolveValue CreateListResolver(ResolverContext resolverContext);
        IResolveValue CreateRedirectingConfigurationResolver(ResolverContext resolverContext);
    }
}