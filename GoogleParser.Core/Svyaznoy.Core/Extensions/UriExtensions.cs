using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Svyaznoy.Core
{
    public static class UriExtensions
    {

        public static string GetBaseAddress(this Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return (uri.Scheme == null ? "" : uri.Scheme + "://") + uri.Host + (uri.Port == 80 ? "" : ":"+uri.Port.ToString());
        }

        public static string AddParameters(this Uri uri, IDictionary<string, string> @params)
        {
            var builder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (@params != null)
            {
                foreach (var param in @params)
                {
                    query.Add(param.Key, param.Value);
                }
            }
            builder.Query = query.ToString();
            return builder.ToString();
        }

        public static string AddParameter(this Uri url, string paramName, string paramValue)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }
    }
}
