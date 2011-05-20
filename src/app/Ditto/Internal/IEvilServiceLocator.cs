namespace Ditto.Internal
{
    public interface IEvilServiceLocator
    {
        T Get<T>();
    }
}