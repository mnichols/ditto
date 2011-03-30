using Ditto.Internal;

namespace Ditto.Resolvers
{
    public interface ICreateResolver
    {
        ListResolver CreateListResolver(ResolverContext resolverContext);
        RedirectingConfigurationResolver CreateRedirectingConfigurationResolver(ResolverContext resolverContext);
    }
}