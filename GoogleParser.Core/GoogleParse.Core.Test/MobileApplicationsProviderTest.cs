using System;
using System.Configuration;
using System.Linq;
using GoogleParser.Core.Entities;
using GoogleParser.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleParse.Core.Test
{
    [TestClass]
    public class MobileApplicationsProviderTest
    {
        private string _connectionString;
        private MobileApplicationsProvider _provider;

        [TestInitialize]
        public void Init()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MarketApplicationsEntities"].ConnectionString;
            _provider = new MobileApplicationsProvider(_connectionString);
        }

        [TestMethod]
        public void CreateAppCard()
        {
            ParsedAppCard parsedCard = new ParsedAppCard
            {
                Url = "http://play.google.com/store/apps/details?id=com.glu.kandk",
                DeveloperEmail = "androidsupport@glu.com",
                DeveloperName = "Glu",
                Name = "KENDALL & KYLIE"
            };

            _provider.Save(parsedCard);

            var savedCards = _provider.GetCards(lastUpdated: new DateTime(2016, 02, 18));

            Assert.IsNotNull(savedCards);
            Assert.IsTrue(savedCards.Count() == 1);

            var savedCard = savedCards.FirstOrDefault(c => c.Url == parsedCard.Url);

            Assert.IsTrue(parsedCard.Url == savedCard.Url);
            Assert.IsTrue(parsedCard.DeveloperEmail == savedCard.DeveloperEmail);
            Assert.IsTrue(parsedCard.DeveloperName == savedCard.DeveloperName);
            Assert.IsTrue(parsedCard.Name == savedCard.Name);
        }

        [TestMethod]
        public void UpdateAppCard()
        {
            ParsedAppCard parsedCard = new ParsedAppCard
            {
                Url = "http://play.google.com/store/apps/details?id=com.glu.kandk",
                DeveloperEmail = Guid.NewGuid() + "@glu.com",
                DeveloperName = "Glu",
                Name = "KENDALL & KYLIE"
            };

            _provider.Save(parsedCard);

            var savedCards = _provider.GetCards(lastUpdated: new DateTime(2016, 02, 18));

            Assert.IsNotNull(savedCards);
            Assert.IsTrue(savedCards.Count() == 1);

            var savedCard = savedCards.FirstOrDefault(c => c.Url == parsedCard.Url);

            Assert.IsTrue(parsedCard.Url == savedCard.Url);
            Assert.IsTrue(parsedCard.DeveloperEmail == savedCard.DeveloperEmail);
            Assert.IsTrue(parsedCard.DeveloperName == savedCard.DeveloperName);
            Assert.IsTrue(parsedCard.Name == savedCard.Name);
        }
    }
}
 