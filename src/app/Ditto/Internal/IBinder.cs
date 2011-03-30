namespace Ditto.Internal
{
    public interface IBinder
    {
        void Bind(BindableConfiguration bindableConfiguration, params ICreateExecutableMapping[] configurations);
    }
}