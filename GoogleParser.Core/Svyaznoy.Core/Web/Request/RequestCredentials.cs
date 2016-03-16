using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Web.Request
{
    public class RequestCredentials
    {
        public string User { get; set; }

        public string Password { get; set; }

        public bool NTLMAuthorization { get; set; }
    }
}