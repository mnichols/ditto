using System;

namespace Ditto.Internal
{
    public class DefaultMapCommand : IMapCommand
    {
        private readonly IContextualizeResolution contextualizer;
        private readonly IExecuteMapping executableMapping;

        public DefaultMapCommand(IExecuteMapping executableMapping, IContextualizeResolution contextualizer)
        {
            this.executableMapping = executableMapping;
            this.contextualizer = contextualizer;
        }

        public TDest Map<TDest>(object source)
        {
            return (TDest) From(source);
        }

        public TDest Map<TDest>(object source, TDest dest)
        {
            return (TDest) From(source, dest);
        }


        public object From(object source)
        {
            var context = contextualizer.CreateContext(source, executableMapping.DestinationType);
            executableMapping.Execute(context);
            return context.Destination;
        }


        public object From(object source, object destination)
        {
            var context = contextualizer.CreateContext(source, destination);
            executableMapping.Execute(context);
            return destination;
        }
    }
}