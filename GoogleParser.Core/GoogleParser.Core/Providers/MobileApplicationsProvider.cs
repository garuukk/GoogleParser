using System;
using System.Collections.Generic;
using GoogleParser.Core.Accessors;
using GoogleParser.Core.Entities;
using GoogleParser.Core.Model;

namespace GoogleParser.Core.Providers
{
    public class MobileApplicationsProvider
    {
        private string _connectionString;

        private AppCardAccessor _appCardAccessor;

        public MobileApplicationsProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

            _appCardAccessor = new AppCardAccessor();
            _connectionString = connectionString;
        }

        public void Save(ParsedAppCard appCard)
        {
            using (var inventory = new Inventory(_connectionString))
            {
                _appCardAccessor.SaveCard(inventory, appCard);
            }
        }

        public IEnumerable<ApplicationCard> GetCards(List<string> links = null, List<Guid> ids = null, List<string> names= null, DateTime? lastUpdated = null)
        {
            using (var inventory = new Inventory(_connectionString))
            {
                return _appCardAccessor.GetCards(inventory, links: links, ids: ids, names: names, lastUpdated: lastUpdated);
            }
        }
    }
}
