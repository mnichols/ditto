namespace Ditto.Internal
{
    public interface IToggle<TDest>
    {
        void By<TTrue, TFalse>();
    }
}