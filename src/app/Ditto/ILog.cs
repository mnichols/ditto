namespace Ditto
{
    public interface ILog
    {
        void Debug(string message,params object[] args);
        void Warn(string message, params object[] args);
        void Info(string message, params object[] args);

    }
}