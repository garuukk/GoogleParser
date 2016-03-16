using System;
using System.Linq;

namespace Svyaznoy.Core.Web.Model
{
    public class PublicException : Exception
    {
        public PublicException(string userMessage)
            : this(userMessage, null)
        {
        }

        public PublicException(string userMessage, Exception ex)
            : base(userMessage, ex)
        {
        }
    }
}