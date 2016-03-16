using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using Svyaznoy.Core.Log;

namespace Svyaznoy.Core.Model
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Executes query with limit and query for total count if required
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IQueryable<T> Limit<T>(this IQueryable<T> query, Limit limit)
        {
            if (limit == null)
                return query;

            if(limit.RequireTotalCount)
                limit.TotalCount = query.Count();

            if (limit.CountToSkip > 0)
                query = query.Skip(limit.CountToSkip);
            if (limit.CountToTake > 0)
                query = query.Take(limit.CountToTake);

            return query;
        }


        /// <summary>
        /// Executes query with nolock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> ToListReadUncommited<T>(this IQueryable<T> query)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                                              new TransactionOptions()
                                                                  {
                                                                      IsolationLevel =
                                                                          System.Transactions.IsolationLevel.ReadUncommitted
                                                                  }))
            {
                List<T> toReturn = query.ToList();
                scope.Complete();
                return toReturn;
            }
        }

        /// <summary>
        /// Dealing with unoptimal sql plan cache. If received timeout exception from sql server then trying to remove this sql plan cache and re-execute request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="inventory"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static List<T> ToListWithCacheClearOnTimeout<T>(this IQueryable<T> query, IInventory inventory, ILogger log = null)
        {
            List<T> result = null;
            try
            {
                result = query.ToList();
            }
            catch (Exception ex)
            {
                var sqlException = ex as SqlException;

                if (sqlException == null)
                {
                    var innerException = ex.InnerException;
                    while (innerException!=null)
                    {
                        sqlException = innerException as SqlException;
                        if (sqlException != null)
                            break;
                        innerException = innerException.InnerException;
                    }
                }

                if (sqlException != null && sqlException.Message.ToLower().Contains("timeout"))
                {
                    if(log!=null)
                        log.Error("Received sql timeout exception. Trying to remove sql plan cache", ex);

                    var searchSql = query.ToString()
                        .Replace("\r\n", "%")
                        .Replace("\t", "%")
                        .Replace(' ', '%')
                        .Replace("[", "\\[")
                        .Replace("]", "\\]");

                    var sql =
@"
declare @planHandle varbinary(64);

declare @searchSql nvarchar(max);

set @searchSql = '%{0}%'

set @planHandle = (
select top 1 decp.plan_handle
from sys.dm_exec_cached_plans decp
cross apply sys.dm_exec_sql_text(decp.plan_handle) dest
where dest.text like @searchSql escape '\'
)

if(@planHandle is not null)
	dbcc freeproccache (@planHandle);
";
                    sql = string.Format(sql, searchSql);

                    inventory.ExecuteNonQueryCommand(sql);

                    // try get result once more time
                    result = query.ToList();
                }
                else
                {
                    throw;
                }
            }

            return result ?? new List<T>();
        }
    }
}
