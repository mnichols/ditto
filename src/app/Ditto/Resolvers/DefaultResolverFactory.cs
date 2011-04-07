using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class DefaultResolverFactory:ICreateResolver
    {
        private IActivator activator;

        public DefaultResolverFactory(IActivator activator)
        {
            this.activator = activator;
        }

        public ListResolver CreateListResolver(ResolverContext resolverContext)
        {
            return new ListResolver(resolverContext.SourceProperty,resolverContext.DestinationProperty,resolverContext.Configuration,activator);
        }

        public RedirectingConfigurationResolver CreateRedirectingConfigurationResolver(ResolverContext resolverContext)
        {
            return new RedirectingConfigurationResolver(resolverContext.SourceProperty,resolverContext.Configuration);
        }
    }
}