using System.Collections.Generic;
using System.Data.Entity;

namespace Svyaznoy.Core.Model
{
    public partial class Context : DbContext
    {
        public Context(string connectionString)
            : base(connectionString)
        {
        }

        private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();

        public static Context Create(string sqlConnectionString, string contextMetadata)
        {
            if (!Cache.ContainsKey(sqlConnectionString))
            {
                Cache[sqlConnectionString] = sqlConnectionString;
            }
            
            return new Context(Cache[sqlConnectionString]);
        }
    }
}
