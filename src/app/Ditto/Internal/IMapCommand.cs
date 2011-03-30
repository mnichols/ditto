namespace Ditto.Internal
{
    public interface IMapCommand
    {
        TDest Map<TDest>(object source);
        TDest Map<TDest>(object source, TDest dest);
    }
}