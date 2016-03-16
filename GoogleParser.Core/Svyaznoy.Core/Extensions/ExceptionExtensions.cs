using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Svyaznoy.Core
{
    public static class ExceptionExtensions
    {
        public static string ErrorDump(this Exception ex)
        {
            if (ex == null)
                return null;
            var sb = new StringBuilder();

            if (ex is AggregateException)
            {
                var ae = (AggregateException)ex;
                ae.Flatten().InnerExceptions
                    .Select(e => e.ErrorDump())
                    .ForEach(e => sb.AppendLine(e));
            }
            else
            {
                var stackTrace = ex.StackTrace;
                sb.AppendLine(ex.GetType().FullName);
                sb.AppendLine(ex.Message);

                ex = ex.InnerException;
                while (ex != null)
                {
                    sb.AppendLine("--- Inner Exception ---");
                    //sb.AppendLine(ex.GetType().FullName);
                    sb.AppendLine(ex.ToString());
                    ex = ex.InnerException;
                }

                sb.AppendLine("--- Stack Trace ----");
                sb.AppendLine(stackTrace);
            }

            return sb.ToString();
        }

        public static string GetCallerMethodName(int addLevelsUp = 0)
        {
            var trace = new StackTrace();

            var method = trace.GetFrame(2 + addLevelsUp).GetMethod();
            return method == null || method.DeclaringType == null
                       ? ""
                       : method.DeclaringType.FullName + "." + method.Name;
        }

        public static void AddResponseAndThrow(this WebException ex)
        {
            if (ex.Response != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseString = reader.ReadToEnd();
                    throw new WebException(ex.Message + "\n\n" + responseString, ex);
                }
            }
            else
                throw ex;
        }
    }
}