using System.IO;
using log4net;

namespace Svyaznoy.Core.Log
{
    public class LoggerAppConfig : ALogger
    {
        public LoggerAppConfig()
        {
            Log = LogManager.GetLogger(typeof (LoggerAppConfig));
            
            log4net.Config.XmlConfigurator.Configure();
        }

        public LoggerAppConfig(FileInfo configFile)
        {
            Log = LogManager.GetLogger(typeof(LoggerAppConfig));
            log4net.Config.XmlConfigurator.Configure(configFile);
        }
    }
}

    