using Castle.Core.Logging;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Ditto.Tests
{
    public class WithDebugging
    {
        static WithDebugging()
        {
            BasicConfigurator.Configure(new DebugAppender
            {
                Threshold = Level.Debug,
                Layout = new SimpleLayout()
            });
            //Log.InitializeLogFactory(new ConsoleFactory());
        }
    }
}