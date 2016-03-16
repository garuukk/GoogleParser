using GoogleParser.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleParse.Core.Test
{
    [TestClass]
    public class RequestHelperTest
    {
        RequestHelper requestHelper = new RequestHelper();

        [TestMethod]
        public void SendByAwesomium_Test()
        {
            string url = "https://play.google.com/store/apps/collection/topselling_free";

            requestHelper.SendRequestByAvesomium(url, delegate(string html, string processedUrl)
            {
                Assert.IsNotNull(html);
                Assert.Equals(processedUrl, url);
            });
        }

        [TestMethod]
        public void SendByOwnerSender_Test()
        {
            string url = "https://play.google.com/store/apps/collection/topselling_free";
            string response = requestHelper.SendRequest(url);

            Assert.IsNotNull(response);
        }
    }
}