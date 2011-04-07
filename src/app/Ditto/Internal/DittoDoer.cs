namespace Ditto.Internal
{
    public class DittoDoer : IMap
    {
        private ICreateMappingCommand commands;

        public DittoDoer(ICreateMappingCommand commands)
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
    }
}