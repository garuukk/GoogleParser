using System;
using System.IO;
using System.Linq;
using GoogleParser.Core.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleParse.Core.Test
{
    [TestClass]
    public class GoogleMarketParserTest
    {
        private IMarketParser marketParser = new GoogleMarketParse();

        [TestMethod]
        public void CollectionPage_Test()
        {
            string testDataFile = @"GoogleCollectionPage_TestData.txt";
            string testData = ReadTestDataFromFile(testDataFile);

            var cards = marketParser.ParseCollectionPage(testData);

            Assert.IsNotNull(cards);
            Assert.IsTrue(cards.Count() == 120);
        }

        [TestMethod]
        public void AdditionalCollectionPage_Test()
        {
            string testDataFile = @"GoogleCollectionAdditionalPage_TestData.txt";
            string testData = ReadTestDataFromFile(testDataFile);

            var cards = marketParser.ParseCollectionPage(testData);

            Assert.IsNotNull(cards);
            Assert.IsTrue(cards.Count() == 60);
        }

        [TestMethod]
        public void CardPage_Test()
        {
            string testDataFile = @"GoogleCardPage_TestData.txt";
            string testData = ReadTestDataFromFile(testDataFile);
            
            var card = marketParser.ParseAppPage(testData);

            Assert.IsNotNull(card);
            Assert.IsTrue(card.DeveloperEmail == "info@beeline.ru");
            Assert.IsTrue(card.DeveloperName == "ПАО \"ВымпелКом\"");
            Assert.IsNull(card.Url);
            Assert.IsTrue(card.Name == "Мой Билайн");
        }

        private string ReadTestDataFromFile(string fileName)
        {
            string testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);

            string testData = File.ReadAllText(testDataPath);

            return testData;
        }
    }
}
