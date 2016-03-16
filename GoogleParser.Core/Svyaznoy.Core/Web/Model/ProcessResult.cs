using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Web.Model
{
    public class ProcessResult
    {
        public bool HasError { get; set; }

        public string Message { get; set; }

        public string Header { get; set; }

        public static ProcessResult Success(string message = null, string header = null)
        {
            return new ProcessResult()
            {
                HasError = false,
                Message = message,
                Header = header,
            };
        }

        public static ProcessResult Error(string message, string header = null)
        {
            return new ProcessResult()
            {
                HasError = true,
                Message = message,
                Header = header,
            };
        }
    }
}
