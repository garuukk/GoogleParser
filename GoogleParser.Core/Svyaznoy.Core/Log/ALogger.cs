using System;
using System.Diagnostics;
using System.Linq;
using Svyaznoy.BIP.Utils;
using log4net;

namespace Svyaznoy.Core.Log
{
    public abstract class ALogger : ILogger
    {
        protected ILog Log;

        protected ALogger()
        {
        }

        public void Info(string message, params object[] format)
        {
            Log.InfoFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message, format);
        }

        public void Info(string message, Exception exception, params object[] format)
        {
            Log.InfoFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message + "\n" + exception.ErrorDump(), format);
        }

        public void Debug(string message, params object[] format)
        {
            Log.DebugFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message, format);
        }

        public void Debug(string message, Exception exception, params object[] format)
        {
            Log.DebugFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message + "\n" + exception.ErrorDump(), format);
        }

        public void Warn(string message, params object[] format)
        {
            Log.WarnFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message, format);
        }

        public void Warn(string message, Exception exception, params object[] format)
        {
            Log.WarnFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message + "\n" + exception.ErrorDump(), format);
        }

        public void Error(string message, params object[] format)
        {
            Log.ErrorFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message, format);
        }

        public void Error(string message, Exception exception, params object[] format)
        {
            Log.ErrorFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message + "\n" + exception.ErrorDump(), format);
        }

        public void Fatal(string message, params object[] format)
        {
            Log.FatalFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message, format);
        }

        public void Fatal(string message, Exception exception, params object[] format)
        {
            Log.FatalFormat(ExceptionExtensions.GetCallerMethodName() + ": " + message + "\n" + exception.ErrorDump(), format);
        }

       

    }
}
