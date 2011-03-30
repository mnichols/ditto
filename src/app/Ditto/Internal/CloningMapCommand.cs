namespace Ditto.Internal
{
    public class CloningMapCommand : IMapCommand
    {
        private readonly IMapCommand inner;
        private readonly IReflection reflection;

        public CloningMapCommand(IReflection reflection, IMapCommand inner)
        {
            this.inner = inner;
            this.reflection = reflection;
        }

        public TDest Map<TDest>(object source)
        {
            return inner.Map<TDest>(reflection.Copy(source));
        }

        public TDest Map<TDest>(object source, TDest dest)
        {
            return inner.Map(reflection.Copy(source), dest);
        }
    }
}