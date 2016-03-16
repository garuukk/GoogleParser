using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Awesomium.Core;

namespace GoogleParser.Core.Helpers
{
    public class RequestHelper : IRequestHelper
    {
        const string PAGE_HEIGHT_FUNC = "(function() { " +
            "var bodyElmnt = document.body; " +
            "var html = document.documentElement; " +
            "var height = Math.max( bodyElmnt.scrollHeight, bodyElmnt.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); " +
            "return height; })();";

        private const string FIND_MORE_BUTTON = "(function() { " +
                                                "var moreButton = document.getElementById('show-more-button');" +
                                                "if(moreButton == null || moreButton.style.display == 'none') return false;" +
                                                "else { moreButton.click(); return true; }" +
                                                " })();";

        private int _lastDocHeight = 0;

        private WebSession _session;
        private WebView _view;

        private Action<string, string> _isLoadedAction;
        private string _currentUrl;
        private bool _enableTimeout;

        public RequestHelper()
        {
            _session = WebCore.CreateWebSession(new WebPreferences {CustomCSS = "::-webkit-scrollbar { visibility: hidden; }"});
            _view = WebCore.CreateWebView(1100, 600, _session);

            _view.LoadingFrameComplete += (s, e) =>
            {
                if (!e.IsMainFrame)
                    return;

                int docHeight = (int)_view.ExecuteJavascriptWithResult(PAGE_HEIGHT_FUNC);
                bool scrollDown = true;

                while (scrollDown)
                {
                    var jsResult = _view.ExecuteJavascriptWithResult("window.scrollTo(100," + docHeight + ");");
                    _lastDocHeight = docHeight;

                    if (_enableTimeout)
                        Thread.Sleep(4000);

                    docHeight = (int)_view.ExecuteJavascriptWithResult(PAGE_HEIGHT_FUNC);

                    if (_lastDocHeight >= docHeight)
                    {
                        var click = _view.ExecuteJavascriptWithResult(FIND_MORE_BUTTON);
                        if (click)
                        {
                            docHeight = (int)_view.ExecuteJavascriptWithResult(PAGE_HEIGHT_FUNC);
                        }
                        else
                        {
                            scrollDown = false;
                        }
                    }

                    //
                }

                var html = _view.ExecuteJavascriptWithResult("document.getElementsByTagName('html')[0].innerHTML");

                if (_isLoadedAction != null)
                    _isLoadedAction.Invoke(html, _currentUrl);
            };
        }

        public void SendRequestByAvesomium(string url, Action<string, string> isLoaded, bool enableTimeout = false)
        {
            _currentUrl = url;
            _enableTimeout = enableTimeout;
            _isLoadedAction = isLoaded;
            _lastDocHeight = 0;

            var uri = new Uri(url);
            _view.Source = uri;

            if (WebCore.UpdateState == WebCoreUpdateState.NotUpdating)
            {
                WebCore.Run();
            }
        }

        public string SendRequest(string url, string webRequestMethod = WebRequestMethods.Http.Get, string parameters = null)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = webRequestMethod;

            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.KeepAlive = true;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            if (webRequestMethod == WebRequestMethods.Http.Post)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(parameters ?? string.Empty);
                webRequest.ContentLength = bytes.Length;
                webRequest.AllowWriteStreamBuffering = true;
                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            string responseData;

            var response = webRequest.GetResponse();

            using (Stream stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                responseData = reader.ReadToEnd();
            }

            return responseData;
        }
    }
}
