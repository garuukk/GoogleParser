using System;
using System.Collections.Generic;
using System.Linq;
using GoogleParser.Core.Entities;
using Svyaznoy.Core;
using Svyaznoy.Core.Model;

namespace GoogleParser.Core.Accessors
{
    public class AppCardAccessor
    {
        public void SaveCard(IHistoryDataInventory inventory, ParsedAppCard card)
        {
            var dbItem = inventory.SelectNotDeleted<DbAppCard>().FirstOrDefault(c => c.Url == card.Url);

            if (dbItem == null)
            {
                CreateCard(inventory, card);
            }
            else
            {
                if (dbItem.Name != card.Name || dbItem.DeveloperEmail != card.DeveloperEmail ||
                    dbItem.DeveloperName != card.DeveloperName ||
                    dbItem.DeveloperPhysicalAddress != card.DeveloperPhysicalAddress)
                {
                    dbItem.UpdatedUtcDate = DateTime.UtcNow;
                }

                dbItem.Name = card.Name;
                dbItem.DeveloperEmail = card.DeveloperEmail;
                dbItem.DeveloperName = card.DeveloperName;
                dbItem.DeveloperPhysicalAddress = card.DeveloperPhysicalAddress;
            }

            inventory.SubmitChangesWithLog(Guid.Empty);
        }

        private void CreateCard(IHistoryDataInventory inventory, ParsedAppCard card)
        {
            if (card == null) 
                return;

            var dbItem = new DbAppCard();
            
            dbItem.Id = Guid.NewGuid();
            dbItem.Url = card.Url;
            dbItem.Name = card.Name;
            dbItem.DeveloperEmail = card.DeveloperEmail;
            dbItem.DeveloperName = card.DeveloperName;
            dbItem.DeveloperPhysicalAddress = card.DeveloperPhysicalAddress;
            dbItem.CreatedUtcDate = DateTime.UtcNow;

            inventory.CreateOnSubmit(dbItem);
            inventory.SubmitChangesWithLog(Guid.Empty);
        }

        public IEnumerable<ApplicationCard> GetCards(IHistoryDataInventory inventory, List<string> links = null, List<Guid> ids = null, List<string> names = null, DateTime? lastUpdated = null)
        {
            bool hasLinks = links.HasValues();
            bool hasIds = ids.HasValues();
            bool hasNames = names.HasValues();
            bool hasLastUpdated = lastUpdated.HasValue;

            var q = inventory.Select<DbAppCard>();

            if (hasLinks)
            {
                q = q.Where(c => links.Contains(c.Url));
            }

            if (hasIds)
            {
                q = q.Where(c => ids.Contains(c.Id));
            }

            if (hasNames)
            {
                q = q.Where(c => names.Contains(c.Name));
            }

            if (hasLastUpdated)
            {
                q =
                    q.Where(
                        c =>
                            (c.UpdatedUtcDate != null && c.UpdatedUtcDate > lastUpdated) ||
                            (c.UpdatedUtcDate == null && c.CreatedUtcDate > lastUpdated));
            }

            var res = q.ToList();

            return res.Select(MapDbAppCard).Where(c => c != null).ToList();
        }

        #region Map

        public static ApplicationCard MapDbAppCard(DbAppCard dbItem)
        {
            if (dbItem == null) return null;

            var item = new ApplicationCard();

            item.Id = dbItem.Id;
            item.Name = dbItem.Name;
            item.Url = dbItem.Url;
            item.DeveloperName = dbItem.DeveloperName;
            item.DeveloperEmail = dbItem.DeveloperEmail;
            item.DeveloperPhysicalAddress = dbItem.DeveloperPhysicalAddress;
            item.CreatedUtcDate = dbItem.CreatedUtcDate;
            item.UpdatedUtcDate = dbItem.UpdatedUtcDate;

            return item;
        }

        #endregion
    }
}

