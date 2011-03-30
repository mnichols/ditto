namespace Ditto.Internal
{
    public class DefaultMappingEngine : IMap
    {
        private ICreateMappingCommand commands;

        public DefaultMappingEngine(ICreateMappingCommand commands)
        {
            this.commands = commands;
        }


        public TDest Map<TDest>(object source)
        {
            var destination = commands.CreateCommand(typeof (TDest), source.GetType());
            return destination.Map<TDest>(source);
        }

        public TDest Map<TDest>(object source, TDest dest)
        {
            var destination = commands.CreateCommand(typeof (TDest), source.GetType());
            return destination.Map(source, dest);
        }

        public void Assert()
        {
            var validatable = commands as IValidatable;
            if (validatable != null)
                validatable.Assert();
        }
    }
}