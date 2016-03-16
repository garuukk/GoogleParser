using System;
using System.Net;

namespace GoogleParser.Core.Helpers
{
    public interface IRequestHelper
    {
        void SendRequestByAvesomium(string url, Action<string, string> isLoaded, bool enableTimeout = false);

        string SendRequest(string url, string webRequestMethod = WebRequestMethods.Http.Get, string parameters = null);
    }
}
