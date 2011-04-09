using Ditto.Resolvers;

namespace Ditto.Internal
{
    public class BinderFactory:IProvideBinders
    {
        private readonly IActivate activate;

        public BinderFactory(IActivate activate)
        {
            this.activate = activate;
            Logger = new NullLogFactory();
        }

        public ILogFactory Logger { get; set; }
        public IBinder[] Create()
        {
            var resolverFactory = new DefaultResolverFactory(activate);
            var destinationPropsBinding = new DestinationPropertyTypeBinder(resolverFactory){Logger=Logger};
            var listPropBinding = new ListPropertyBinder(resolverFactory){Logger=Logger};
            var selfBinding = new SelfBinder();
            return new IBinder[]{destinationPropsBinding,listPropBinding,selfBinding};
        }
    }
}