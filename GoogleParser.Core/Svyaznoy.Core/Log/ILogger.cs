using System;

namespace Svyaznoy.Core.Log
{
    public interface ILogger
    {
        void Info(String message, params object[] format);

        void Info(String message, Exception exception, params object[] format);

        void Debug(String message, params object[] format);

        void Debug(String message, Exception exception, params object[] format);

        void Warn(String message, params object[] format);

        void Warn(String message, Exception exception, params object[] format);

        void Error(String message, params object[] format);

        void Error(String message, Exception exception, params object[] format);

        void Fatal(String message, params object[] format);

        void Fatal(String message, Exception exception, params object[] format);
    }
}