using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Svyaznoy.Core.Web.Model;

namespace Svyaznoy.Core.Web
{
    public static class WebExtension
    {
        public static void SetSession(this HttpContextBase context, string key, object value)
        {
            if (context == null)
                return;

            if (context.Session != null)
            {
                DeleteSession(context, key);
                context.Session[key] = value;
            }
        }

        public static object GetSession(this HttpContextBase context, string key)
        {
            if (context != null
                && context.Session != null
                && context.Session.Count != 0)
            {
                if (context.Session[key] != null)
                    return context.Session[key];
                return null;
            }
            return null;
        }

        /// <summary>
        /// статический метод получения cookies клиента
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nameCookies">
        /// название cookies
        /// </param>
        /// <returns>
        /// содержимое cookies в виде строки
        /// </returns>
        public static string GetCookies(this HttpContextBase context, string nameCookies)
        {
            if (context == null)
                return null;
            try
            {
                var httpCookie = context.Request.Cookies[nameCookies];
                return GetCookieValue(httpCookie);
            }
            catch
            {
                //If someone is trying to spoof, do it again.
                return null;
            }
        }

        private static string GetCookieValue(HttpCookie httpCookie)
        {
            if (httpCookie != null && httpCookie.Value != null)
            {
                var isHex = httpCookie.Value.All(c => HexDigits.Contains(c));

                if (isHex)
                {
                    var ticket = FormsAuthentication.Decrypt(httpCookie.Value);

                    if (ticket == null || ticket.Expired)
                    {
                        return null;
                    }

                    return ticket.UserData;
                }
                else
                {
                    return httpCookie.Value;
                }
            }
            return null;
        }

        private static readonly char[] HexDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// статический метод добавления cookie клиенту
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cookieName">
        /// название cookie
        /// </param>
        /// <param name="cookieValue">
        /// значение cookie
        /// </param>
        /// <returns>
        /// содержимое cookie в виде строки
        /// </returns>
        public static bool SetCookies(this HttpContextBase context, string cookieName, string cookieValue)
        {
            if (context == null)
                return false;

            return SetCookies(context, cookieName, cookieValue, 45);
        }

        /// <summary>
        /// статический метод добавления cookie клиента
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cookieName">
        /// название cookie
        /// </param>
        /// <param name="cookieValue">
        /// значение cookie
        /// </param>
        /// <param name="cookieDuration">
        /// длительность действия cookie в минутах
        /// </param>
        /// <returns>
        /// содержимое cookie в виде строки
        /// </returns>
        public static bool SetCookies(this HttpContextBase context, string cookieName, string cookieValue, int cookieDuration)
        {
            if (context == null)
                return false;

            try
            {
                var dateExpires = DateTime.Now.AddMinutes(cookieDuration);
                return SetCookies(context, cookieName, cookieValue, dateExpires);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// статический метод добавления cookie клиенту
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cookieName">
        /// название cookie
        /// </param>
        /// <param name="cookieValue">
        /// значение cookie
        /// </param>
        /// <param name="dateExpires">срок истечения</param>
        /// <returns>
        /// содержимое cookie в виде строки
        /// </returns>
        public static bool SetCookies(this HttpContextBase context, string cookieName, string cookieValue, DateTime dateExpires)
        {
            if (context == null)
                return false;

            try
            {
                var ticket = new FormsAuthenticationTicket(1,
                  context.User.Identity.Name,
                  DateTime.Now,
                  dateExpires,
                  false,
                  cookieValue,
                  "");

                // шифрование данных.
                var encTicket = FormsAuthentication.Encrypt(ticket);
                // создание cookie.
                context.Response.Cookies.Add(new HttpCookie(cookieName, encTicket)
                {
                    Expires = dateExpires,
                });
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// статический метод удаления сессионной переменной из колекции контента (Session.Contents)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nameSessionItem">
        /// название сессионной переменной
        /// </param>
        public static bool DeleteSession(this HttpContextBase context, string nameSessionItem)
        {
            if (context == null || context.Session == null)
                return false;
            context.Session.Contents.Remove(nameSessionItem);
            return true;
        }

        public static string GetRemoteIP(this HttpContextBase context)
        {
            if (context == null)
                return null;

            var ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                ip = ip.Split(',')[0];
            }
            return IpToInt(ip) != 0 ? ip : "127.0.0.1";
        }

        public static string GetRequestParameter(this HttpContextBase context, string name)
        {
            if (context == null || string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim().ToLower();

            var keys = context.Request.Params.AllKeys.Where(k => k.ToLower() == name).ToList();

            if (keys.IsNullOrEmpty())
            {
                return null;
            }

            return context.Request.Params[keys[0]];
        }

        public static long IpToInt(string ip)
        {
            if (ip == "::1") // IPv6 localhost
            {
                return 0;
            }
            long ipInt = 0;
            try
            {
                if (ip.Contains('.'))
                {
                    var uIP = ip.Split(new Char[] { '.' });
                    long step = 1;
                    for (var i = 0; i < uIP.Length; i++)
                    {
                        ipInt = ipInt + Convert.ToInt32(uIP[uIP.Length - 1 - i]) * step;
                        step = step * 256;
                    }
                }
            }
            catch
            { }
            return ipInt;
        }

        public static bool IsLocalIpAddress(string address)
        {
            var ip = IpToInt(address);

            if (ip == 2130706433 ||                       //    127.0.0.1
                (ip >= 167772160 && ip <= 184549375) ||   // 	10.0.0.0 - 10.255.255.255
                (ip >= 2886729728 && ip <= 2887778303) || //    172.16.0.0 - 172.31.255.255
                (ip >= 3232235520 && ip <= 3232301055)    // 	192.168.0.0 - 192.168.255.255
                )
                return true;
            else
            {
                return false;
            }
        }

        public static string CreateErrorDump(this HttpRequestBase request, Exception error = null)
        {
            if (request == null)
                return null;
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(error == null ? null : error.ToString());
            sb.AppendLine();
            if (request != null)
            {
                sb.AppendLine("Url:           " + request.RawUrl);
                sb.AppendLine("Url referrer:  " +
                              (request.UrlReferrer == null ? null : request.UrlReferrer.ToString()));
                sb.AppendLine("Client IP:     " + request.UserHostAddress);
                sb.AppendLine("Client Browser:" + (request.UserAgent == null ? null : request.UserAgent.ToString()));

                sb.AppendLine();
                sb.AppendLine("Cookies:    ");
                sb.AppendLine();

                if (request.Cookies != null)
                {
                    foreach (var cookieKey in request.Cookies.AllKeys)
                    {
                        var cookie = request.Cookies[cookieKey];

                        var val = cookie.Value;
                        try
                        {
                            // try to decrypt
                            val = GetCookieValue(cookie);
                        }
                        catch { }

                        sb.AppendLine("\t" + cookieKey + ": " + val);
                    }
                }

                sb.AppendLine();
                sb.AppendLine("Params:   ");
                sb.AppendLine();
                if (request.Params != null)
                {
                    foreach (var param in request.Params.AllKeys)
                    {
                        sb.AppendLine(string.Format("    {0}            {1}", param, request.Params[param]));
                    }
                }
            }
            sb.AppendLine();

            return sb.ToString();
        }

        public static string GetMimeType(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var pointIndex = fileName.LastIndexOf('.');
            if (pointIndex < 0 || pointIndex == fileName.Length - 1)
                return MimeType.Get("");
            else
            {
                return MimeType.Get(fileName.Substring(pointIndex + 1));
            }
        }

        public static string ToJavaScriptString(this string s)
        {
            return (s ?? "").Trim().Replace("'", "&apos;");
        }

        public static string ToHtmlEmbeddableString(this string s)
        {
            return (s ?? "").Trim().Replace("'", "&apos;").Replace("\n", "<br />");
        }

        public static string RemoveHtmlMarkup(this string s)
        {
            if (s == null)
                return null;
            var s1 = Regex.Replace(s, @"<[^>]+>|&nbsp;", "").Trim();
            return Regex.Replace(s1, @"\s{2,}", " ");
        }

        public static string GetAppSetting(string name)
        {
            var value = ConfigurationManager.AppSettings[System.Environment.MachineName + "." + name];
            if (value != null)
                return value;
            else
            {
                return ConfigurationManager.AppSettings[name] ?? "";
            }
        }

        public static ActionResult JsonResult(this HttpRequestBase request, object @object)
        {
            return new JsonResult()
            {
                ContentType = "application/json",
                ContentEncoding = System.Text.Encoding.UTF8,
                Data = @object,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}