using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class BinderFactory:IProvideBinders
    {
        private readonly IActivator activator;

        public BinderFactory(IActivator activator)
        {
            this.activator = activator;
            Logger = new NullLogFactory();
        }

        public ILogFactory Logger { get; set; }
        public IBinder[] Create()
        {
            var resolverFactory = new DefaultResolverFactory(activator);
            var destinationPropsBinding = new DestinationPropertyTypeBinder(resolverFactory){Logger=Logger};
            var listPropBinding = new ListPropertyBinder(resolverFactory){Logger=Logger};
            var selfBinding = new SelfBinder();
            return new IBinder[]{destinationPropsBinding,listPropBinding,selfBinding};
        }
    }
}