using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Svyaznoy.Core.Web.Request
{
    public static class WebRequestHelper
    {
        public static HttpWebRequest CreateWebRequest(string serviceUrl, HttpMethod method,
            NameValueCollection requestParams,
            Encoding requestEncoding = null, RequestCredentials credentials = null,
            bool needUrlEncoding = false, RequestContentType contentType = RequestContentType.FormUrlEncoded,
            int timeout = 30000, bool bodyInBytes = false)
        {
            requestEncoding = requestEncoding ?? Encoding.UTF8;
            var requestParametersString = GetRequestParametersString(requestParams, needUrlEncoding, requestEncoding);
            return CreateWebRequest(serviceUrl, method, requestParametersString, requestEncoding: requestEncoding,
                credentials: credentials,
                needUrlEncoding: needUrlEncoding,
                contentType: contentType,
                timeout: timeout,
                bodyInBytes: bodyInBytes);
        }

        public static HttpWebRequest CreateWebRequest(string serviceUrl, HttpMethod method, string parameters,
                                                      Encoding requestEncoding = null, RequestCredentials credentials = null,
                                                      bool needUrlEncoding = false, RequestContentType contentType = RequestContentType.FormUrlEncoded,
                                                      int timeout = 30000, bool bodyInBytes = false)
        {
            requestEncoding = requestEncoding ?? Encoding.UTF8;
            var requestParametersString = parameters??"";

            HttpWebRequest request;

            switch (method)
            {
                case HttpMethod.Post:
                    {
                        request = (HttpWebRequest) WebRequest.Create(serviceUrl);
                        request.Method = WebRequestMethods.Http.Post;

                        if (bodyInBytes)
                        {
                            var requestParametersBytes = requestEncoding.GetBytes(requestParametersString);
                            request.ContentLength = requestParametersBytes.Length;
                            using (var requestWriter = request.GetRequestStream())
                            {
                                requestWriter.Write(requestParametersBytes, 0, requestParametersBytes.Length);
                                requestWriter.Close();
                            }

                        }
                        else
                        {
                            request.ContentLength = requestParametersString.Length;
                            using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                            {
                                requestWriter.Write(requestParametersString);
                                requestWriter.Close();
                            }
                        }

                        break;
                    }
                case HttpMethod.Get:
                default:
                    request = (HttpWebRequest)WebRequest.Create(serviceUrl + "?" + requestParametersString);
                    request.Method = WebRequestMethods.Http.Get;
                    break;
            }



            request.ContentType = GetRequestContentType(contentType);
            request.Timeout = timeout;

            if (credentials!=null)
            {
                if (credentials.NTLMAuthorization)
                {
                    var credential = new NetworkCredential(credentials.User, credentials.Password);
                    var credentialCache = new CredentialCache();
                    credentialCache.Add(request.Address, "NTLM", credential);
                    credentialCache.Add(request.Address, "Basic", credential);
                    credentialCache.Add(request.Address, "Digest", credential);
                    request.Credentials = credentialCache;

                }
                else
                {
                    var credential = new NetworkCredential(credentials.User, credentials.Password);
                    request.Credentials = credential;
                }
            }

            request.Proxy = GetRequestProxy(request.Proxy);

            return request;
        }

        

        private static readonly Encoding SrcEncoding = Encoding.Default;


        /// <summary>
        /// Создает список строк из параметров запроса
        /// Каждая строка вида: имя_атрибута=значение_атрибута
        /// Пример: phone=9055630232
        /// Объединяет их в единую строку с разделителем &
        /// </summary>
        /// <param name="reqParams">Коллекция параметров</param>
        /// <param name="needUrlEncoding">Требуется ли url-encoded строка</param>
        /// <param name="targetEncoding">Необходимая кодировка данных для запроса</param>
        /// <returns>Полная строка запроса объединенная &</returns>
        public static string GetRequestParametersString(NameValueCollection reqParams, bool needUrlEncoding, Encoding targetEncoding)
        {
            var lst = new List<string>();
            foreach (string param in reqParams.Keys)
            {
                if (!string.IsNullOrEmpty(reqParams[param]))
                {
                    lst.Add(needUrlEncoding
                                ? string.Format("{0}={1}", param, UrlEncode(reqParams[param], targetEncoding))
                                : string.Format("{0}={1}", param, reqParams[param]));
                }
            }

            if (!needUrlEncoding && targetEncoding != SrcEncoding)
            {
                return targetEncoding.GetString(ConvertEncoding(string.Join("&", lst.ToArray()), targetEncoding));
            }

            return string.Join("&", lst.ToArray());
        }

        /// <summary>
        /// Перекодирует строку в заданную кодировку
        /// </summary>
        /// <param name="srcString">Строка для изменения кодировки</param>
        /// <param name="targetEncoding">Необходимая кодировка</param>
        /// <returns>Массив байт строки в заданной кодировке</returns>
        private static byte[] ConvertEncoding(string srcString, Encoding targetEncoding)
        {
            return Encoding.Convert(SrcEncoding, targetEncoding, SrcEncoding.GetBytes(srcString));
        }

        /// <summary>
        /// Преобразует строку в url-encoded
        /// </summary>
        /// <param name="srcString">исходная строка</param>
        /// <param name="targetEncoding">Необходимая кодировка</param>
        /// <returns>Возвращает url-encoded строку</returns>
        private static string UrlEncode(string srcString, Encoding targetEncoding)
        {
            return HttpUtility.UrlEncode(ConvertEncoding(srcString, targetEncoding));
        }


        private static string GetRequestContentType(RequestContentType contentType)
        {
            string result = null;
            switch (contentType)
            {
                case RequestContentType.FormUrlEncoded:
                    result = "application/x-www-form-urlencoded";
                    break;
            }

            return result;
        }

        public static string GetHttpErrorDescription(HttpWebResponse response)
        {
            return string.Format("Web server returns status code {0}, {1}", response.StatusCode, response.StatusDescription);
        }

        /// <summary>
        /// Получает параметры прокси в зависимости от типа машины (сервер/рабочая станция). Если РЧ, то прокси есть. Для сервера нет.
        /// </summary>
        /// <returns>Сформированный объект с прокси и авторизационными данными, если прокси есть, null, если прокси нет</returns>
        public static IWebProxy GetRequestProxy(IWebProxy requestProxy)
        {
            IWebProxy proxy = requestProxy;

            var machineName = System.Environment.MachineName;
            if (machineName.Length > 7)
            {
                if (machineName.Substring(0, 7).ToLower() == "ws-msk-")
                {
                    proxy = System.Net.WebRequest.GetSystemWebProxy();
                    proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }
            }

            return proxy;
        }
    }
}