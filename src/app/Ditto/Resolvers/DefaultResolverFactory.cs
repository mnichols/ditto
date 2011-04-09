using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class DefaultResolverFactory:ICreateResolver
    {
        private IActivate activate;

        public DefaultResolverFactory(IActivate activate)
        {
            this.activate = activate;
        }

        public ListResolver CreateListResolver(ResolverContext resolverContext)
        {
            return new ListResolver(resolverContext.SourceProperty,resolverContext.Configuration,activate);
        }

        public RedirectingConfigurationResolver CreateRedirectingConfigurationResolver(ResolverContext resolverContext)
        {
            return new RedirectingConfigurationResolver(resolverContext.SourceProperty,resolverContext.Configuration);
        }
    }
}