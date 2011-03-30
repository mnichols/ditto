namespace Ditto.Internal
{
    public interface ICacheable
    {
        void Accept(IVisitCacheable visitor);
    }
}