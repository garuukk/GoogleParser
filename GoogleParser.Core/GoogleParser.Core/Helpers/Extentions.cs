using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GoogleParser.Core.Helpers
{
    public static class Extentions
    {
        public static List<KeyValuePair<string, string>> GetCookies(this WebHeaderCollection headers)
        {
            List<KeyValuePair<string, string>> cookies = null;

            foreach (var headerKey in headers.AllKeys)
            {
                if (headerKey == "Set-Cookie")
                {
                    cookies = Extentions.ParseKeyValuePair(headers[headerKey], ';','=');
                }
            }

            return cookies;
        }

        public static List<KeyValuePair<string, string>> ParseKeyValuePair(string str, char pairSeparator,
            char keyValueSeparator)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var cookies = new List<KeyValuePair<string, string>>();
            var coockiesArray = str.Split(pairSeparator);

            foreach (var cookieTxt in coockiesArray)
            {
                var values = cookieTxt.Split(keyValueSeparator);

                string name = null;
                string value = null;

                if (values.Count() == 1)
                {
                    name = values[0];
                }
                else if (values.Count() == 2)
                {
                    name = values[0];
                    value = values[1];
                }

                var cookie = new KeyValuePair<string, string>(name, value);
                cookies.Add(cookie);
            }

            return cookies;
        }
    }
}
