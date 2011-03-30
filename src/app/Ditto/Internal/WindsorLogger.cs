using Castle.Core.Logging;

namespace Ditto.Internal
{
    public class WindsorLogger:ILog
    {
        private readonly ILogger logger;

        public WindsorLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Debug(string message, params object[] args)
        {
            logger.DebugFormat(message,args);
        }

        public void Warn(string message, params object[] args)
        {
            logger.Warn(message,args);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message,args);
        }

    }
}