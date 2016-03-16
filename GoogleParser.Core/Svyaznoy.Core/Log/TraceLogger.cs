using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Log
{
    public class TraceLogger : ILogger
    {
        private static void WriteLog(string @class, string message, object[] format)
        {
            Trace.WriteLine(@class + " " + string.Format(message, format));
        }

        private static void WriteLog(string @class, string message, Exception exception, object[] format)
        {
            Trace.WriteLine(@class + " " + string.Format(message, format) + "\n" + exception.ErrorDump());
        }

        public void Info(string message, params object[] format)
        {
            WriteLog("Info ", message, format);
        }

        public void Info(string message, Exception exception, params object[] format)
        {
            WriteLog("Info ", message, exception, format);
        }

        public void Debug(string message, params object[] format)
        {
            WriteLog("Debug", message, format);
        }

        public void Debug(string message, Exception exception, params object[] format)
        {
            WriteLog("Debug", message, exception, format);
        }

        public void Warn(string message, params object[] format)
        {
            WriteLog("Warn ", message, format);
        }

        public void Warn(string message, Exception exception, params object[] format)
        {
            WriteLog("Warn ", message, exception, format);
        }

        public void Error(string message, params object[] format)
        {
            WriteLog("Error", message, format);
        }

        public void Error(string message, Exception exception, params object[] format)
        {
            WriteLog("Error", message, exception, format);
        }

        public void Fatal(string message, params object[] format)
        {
            WriteLog("Fatal", message, format);
        }

        public void Fatal(string message, Exception exception, params object[] format)
        {
            WriteLog("Fatal", message, exception, format);
        }
    }
}