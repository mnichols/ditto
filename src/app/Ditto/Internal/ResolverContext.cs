namespace Ditto.Internal
{
    public class ResolverContext
    {
        internal  IDescribeMappableProperty DestinationProperty { get; private set; }    
        internal IDescribeMappableProperty SourceProperty { get; private set; }
        internal ICreateExecutableMapping Configuration { get; private set; }

        public ResolverContext(IDescribeMappableProperty sourceProperty, IDescribeMappableProperty destinationProperty, ICreateExecutableMapping configuration)
        {
            DestinationProperty = destinationProperty;
            SourceProperty = sourceProperty;
            Configuration = configuration;
        }
        public override string ToString()
        {
            return GetType() + " for " +
                "Cfg=" + Configuration + 
                ",DestinationProperty=" + DestinationProperty + 
                ",SourceProperty=" +SourceProperty;
        }

    }
}