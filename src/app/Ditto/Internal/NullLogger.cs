namespace Ditto.Internal
{
    public class NullLogger : ILog
    {
        public void Debug(string message, params object[] args)
        {
            
        }

        public void Warn(string message, params object[] args)
        {
            
        }

        public void Info(string message, params object[] args)
        {
            
        }
    }
}