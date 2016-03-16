using System;
using System.Data;
using System.Data.SqlClient;

namespace Svyaznoy.Core.Model
{
    /// <summary>
    /// Предоставляет функциональные возможности к SQL источнику данных 
    /// </summary>
    public interface ISqlInventory : IDisposable
    {
        /// <summary>
        /// Получить 
        /// </summary>
        /// <param name="sql">SQL запрос</param>
        /// <param name="sqlParameters">параметры запроса</param>
        /// <returns></returns>
        DataTable GetDataTable(string sql, params SqlParameter[] sqlParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readUncommited"></param>
        void BeginTransaction(bool readUncommited = true);

        /// <summary>
        /// 
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DateTime GetSystemDate();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DateTime GetSystemUtcDate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        int ExecuteNonQueryCommand(string sql, CommandType type = CommandType.Text, int? timeout = null,
                                           params SqlParameter[] sqlParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        object ExecuteScalarCommand(string sql, CommandType type = CommandType.Text, int? timeout = null,
                                                    params SqlParameter[] sqlParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <param name="iterateReader"></param>
        /// <param name="sqlParameters"></param>
        void ExecuteReaderCommand(Action<SqlDataReader> reader, string sql, CommandType type = CommandType.Text, int? timeout = null, bool iterateReader = true,
                                                           params SqlParameter[] sqlParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        void Execute(Action<SqlConnection> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        TResult Execute<TResult>(Func<SqlConnection, TResult> function);
    }
}
