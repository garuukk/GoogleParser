using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Log
{
    public class AggregateLogger: ILogger
    {
        private readonly List<ILogger> _loggers = new List<ILogger>();

        public AggregateLogger()
        {
        }

        public AggregateLogger(params ILogger[] loggers)
            : this((IEnumerable<ILogger>)loggers)
        {
        }

        public AggregateLogger(IEnumerable<ILogger> loggers)
        {
            if(loggers!=null)
            _loggers.AddRange(loggers.Where(l=>l!=null));
        }

        public void Add(ILogger logger)
        {
            if(logger!=null)
                _loggers.Add(logger);
        }

        public void Clear()
        {
            _loggers.Clear();
        }

        public void Info(string message, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message, format);
            }
        }

        public void Info(string message, Exception exception, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message, exception, format);
            }
        }

        public void Debug(string message, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(message, format);
            }
        }

        public void Debug(string message, Exception exception, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Debug(message, exception, format);
            }
        }

        public void Warn(string message, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(message, format);
            }
        }

        public void Warn(string message, Exception exception, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Warn(message, exception, format);
            }
        }

        public void Error(string message, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message, format);
            }
        }

        public void Error(string message, Exception exception, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message, exception, format);
            }
        }

        public void Fatal(string message, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(message, format);
            }
        }

        public void Fatal(string message, Exception exception, params object[] format)
        {
            foreach (var logger in _loggers)
            {
                logger.Fatal(message, exception, format);
            }
        }
    }
}
