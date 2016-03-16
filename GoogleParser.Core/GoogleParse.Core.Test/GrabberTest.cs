using System;
using System.Collections.Generic;
using System.Configuration;
using GoogleParser.Core;
using GoogleParser.Core.Entities;
using GoogleParser.Core.Helpers;
using GoogleParser.Core.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Svyaznoy.Core.Log;

namespace GoogleParse.Core.Test
{
    [TestClass]
    public class GrabberTest
    {
        private IMarketParser _marketParser = new GoogleMarketParse();
        private IRequestHelper _requestHelper = new RequestHelper();
        private ILogger _logger = new TraceLogger();
        private string _connectionString;

        [TestInitialize]
        public void Init()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MarketApplicationsEntities"].ConnectionString;
        }

        [TestMethod]
        public void Run_Test()
        {
            Grabber grabber = new Grabber(_marketParser, _requestHelper, _logger, _connectionString);

            var links = new List<string>
            {
                "https://play.google.com/store/apps/collection/promotion_300085a_most_popular_games",
                "https://play.google.com/store/apps/collection/promotion_3000791_new_releases_games"
            };

            grabber.Run(links, ActionRunResponse);
        }

        private void ActionRunResponse(RunResponse runResponse)
        {
                
        }

        [TestMethod]
        public void ExportExcel_Test()
        {
            Grabber grabber = new Grabber(_marketParser, _requestHelper, _logger, _connectionString);

            grabber.ExportToExcel("C:\\temp", new DateTime(2016, 02, 10));
        }
    }
}
