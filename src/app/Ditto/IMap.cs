namespace Ditto
{
    public interface IMap
    {
        TDest Map<TDest>(object source);
        TDest Map<TDest>(object source,TDest dest);
    }
}