using Svyaznoy.Core.Model;

namespace GoogleParser.Core.Model
{
    public class Inventory : HistoryDataInventory<DbRegistratorChanges>
    {
        public Inventory(string connectionString)
            : base(connectionString, "metadata=res://*/MarketApplications.csdl|res://*/MarketApplications.ssdl|res://*/MarketApplications.msl")
        {
        }
    }
}
