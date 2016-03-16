using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace Svyaznoy.Core.Web.Request
{
    public static class UserAgentHelper
    {
        private static readonly Regex AllMobileRegEx =
            new Regex(
                @"android.+mobile|avantgo|bada|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino",
                RegexOptions.IgnoreCase);

        private static readonly Regex WindowsPhoneRegEx = new Regex(@"iemobile|windows (ce|phone)",RegexOptions.IgnoreCase);

        private static readonly Regex AndroidRegEx = new Regex(@"android.+mobile",RegexOptions.IgnoreCase);

        private static readonly Regex IOSRegEx = new Regex(@"ip(hone|od|ad)", RegexOptions.IgnoreCase);

        public static bool IsMobileDevice(this HttpRequestBase request)
        {
            if (request.Browser.IsMobileDevice)
            {
                return true;
            }
            else
            {
                return AllMobileRegEx.IsMatch(request.UserAgent??"");
            }
        }

        public static bool IsAndroidDevice(this HttpRequestBase request)
        {
            return AndroidRegEx.IsMatch(request.UserAgent ?? "");
        }

        public static bool IsIosDevice(this HttpRequestBase request)
        {
            return IOSRegEx.IsMatch(request.UserAgent ?? "");
        }

        public static bool IsWindowsPhoneDevice(this HttpRequestBase request)
        {
            return WindowsPhoneRegEx.IsMatch(request.UserAgent ?? "");
        }
    }
}
