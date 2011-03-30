namespace Ditto.Internal
{
    public interface IBindable
    {
        void Bind(params ICreateExecutableMapping[] configurations);
    }
}