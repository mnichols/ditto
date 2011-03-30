namespace Ditto.Resolvers
{
    internal class OverrideablePropertyNameResolver:PropertyNameResolver,IOverrideable
    {
        public OverrideablePropertyNameResolver(string sourcePropertyName) : base(sourcePropertyName)
        {
        }
    }
}