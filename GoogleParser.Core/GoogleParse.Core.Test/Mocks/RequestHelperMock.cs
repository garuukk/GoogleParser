using System;
using System.IO;
using System.Net;
using GoogleParser.Core.Helpers;

namespace GoogleParse.Core.Test.Mocks
{
    public class RequestHelperMock : IRequestHelper
    {
        private string ReadTestDataFromFile(string fileName)
        {
            string testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);

            string testData = File.ReadAllText(testDataPath);

            return testData;
        }

        public void SendRequestByAvesomium(string url, Action<string, string> isLoaded, bool enableTimeout = false)
        {
            string testDataFile = @"GoogleCollectionAdditionalPage_TestData.txt";
            string testData = ReadTestDataFromFile(testDataFile);

            if(isLoaded != null)
                isLoaded.Invoke(testData, url);
        }

        public string SendRequest(string url, string webRequestMethod = WebRequestMethods.Http.Get, string parameters = null)
        {
            string testDataFile = @"GoogleCollectionPage_TestData.txt";
            string testData = ReadTestDataFromFile(testDataFile);

            return testData;
        }
    }

}
