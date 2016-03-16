using System;
using System.Linq;
using log4net;

namespace Svyaznoy.Core.Log
{
    public class LoggerLog4Net: ALogger
    {
        public LoggerLog4Net(ILog log)
        {
            Log = log;
        }
    }
}
