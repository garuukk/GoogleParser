using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Svyaznoy.Core.Mail;

namespace Svyaznoy.Core.Log
{
    public class SmtpLogger: ILogger
    {
        private readonly ILogger _baseLogger;
        private readonly ServerCredentials _serverCredentials;
        private readonly MailAddress _mailFrom;
        private readonly IEnumerable<MailAddress> _mailTo;
        private readonly string _subject;
        private readonly int _callerLevel;
        private readonly IEnumerable<LogLevel> _levels;

        public SmtpLogger(ILogger baseLogger, IEnumerable<LogLevel> levels, ServerCredentials serverCredentials, MailAddress mailFrom, 
                        IEnumerable<MailAddress> mailTo, string subject, int callerLevel = 0)
        {
            if (levels.IsNullOrEmpty()) throw new ArgumentNullException("levels");
            if (baseLogger == null) throw new ArgumentNullException("baseLogger");
            if (mailFrom == null) throw new ArgumentNullException("mailFrom");
            if (mailTo.IsNullOrEmpty()) throw new ArgumentNullException("mailTo");

            _baseLogger = baseLogger;
            _serverCredentials = serverCredentials;
            _mailFrom = mailFrom;
            _mailTo = mailTo;
            _subject = subject;
            _callerLevel = callerLevel + 1;
            _levels = levels;
        }

        public SmtpLogger(IEnumerable<LogLevel> levels, ServerCredentials serverCredentials, MailAddress mailFrom, IEnumerable<MailAddress> mailTo, string subject, int callerLevel = 0)
            : this(new TraceLogger(), levels, serverCredentials, mailFrom, mailTo, subject, callerLevel)
        {

        }

        private void Send(LogLevel level, string message, Exception exception, params object[] format)
        {
            if (!_levels.Contains(level))
                return;
            string msg = null;

            var sb = new StringBuilder();
            var dt = DateTime.Now;
            sb.AppendLine(dt.ToString("yyyy-MM-dd HH:mm:ss.fff",CultureInfo.InvariantCulture) + " "+ level.ToString());
            sb.AppendLine(ExceptionExtensions.GetCallerMethodName(_callerLevel) + ": " + message);
            if (exception != null)
            {
                sb.AppendLine("---------------------------------");
                sb.AppendLine(exception.ErrorDump());
            }
            msg = sb.ToString();

            try
            {
                EmailAgent.Send(_serverCredentials, new Message()
                        {
                            Body = msg,
                            Subject = _subject + " " + level.ToString(),
                            IsHtml = false
                        }, _mailFrom, _mailTo);
            }
            catch (Exception ex)
            {
                _baseLogger.Error("Smtp log send error", ex);
            }
        }

        public void Info(string message, params object[] format)
        {
            Send(LogLevel.Info, message, null, format);
        }

        public void Info(string message, Exception exception, params object[] format)
        {
            Send(LogLevel.Info, message, exception, format);
        }

        public void Debug(string message, params object[] format)
        {
            Send(LogLevel.Debug, message, null, format);
        }

        public void Debug(string message, Exception exception, params object[] format)
        {
            Send(LogLevel.Debug, message, exception, format);
        }

        public void Warn(string message, params object[] format)
        {
            Send(LogLevel.Warn, message, null, format);
        }

        public void Warn(string message, Exception exception, params object[] format)
        {
             Send(LogLevel.Warn, message, exception, format);
        }

        public void Error(string message, params object[] format)
        {
            Send(LogLevel.Error, message, null, format);
        }

        public void Error(string message, Exception exception, params object[] format)
        {
            Send(LogLevel.Error, message, exception, format);
        }

        public void Fatal(string message, params object[] format)
        {
            Send(LogLevel.Fatal, message, null, format);
        }

        public void Fatal(string message, Exception exception, params object[] format)
        {
            Send(LogLevel.Fatal, message, exception, format);
        }
    }
}
