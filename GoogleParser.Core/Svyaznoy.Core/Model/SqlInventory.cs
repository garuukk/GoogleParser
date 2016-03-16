using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Svyaznoy.Core.Model
{
    public class SqlInventory : ISqlInventory
    {
        private volatile TransactionScope _transactionScope;

        private readonly string _connectionString;

        public string ConnectionString { get { return _connectionString; } }

        protected TransactionScope TransactionScope { get { return _transactionScope; } }

        public SqlInventory(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            _connectionString = connectionString;
        }

        public DataTable GetDataTable(string sql, params SqlParameter[] sqlParameters)
        {
            var dt = new DataTable();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (!sqlParameters.IsNullOrEmpty())
                        cmd.Parameters.AddRange(sqlParameters);
                    using (var r = cmd.ExecuteReader())
                        dt.Load(r);
                }
            }

            return dt;
        }

        public int ExecuteNonQueryCommand(string sql, CommandType type = CommandType.Text, int? timeout = null,
                                     params SqlParameter[] sqlParameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (timeout.HasValue && timeout.Value >= 0)
                        cmd.CommandTimeout = timeout.Value;
                    cmd.CommandType = type;
                    if (!sqlParameters.IsNullOrEmpty())
                        cmd.Parameters.AddRange(sqlParameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalarCommand(string sql, CommandType type = CommandType.Text, int? timeout = null,
                             params SqlParameter[] sqlParameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (timeout.HasValue && timeout.Value >= 0)
                        cmd.CommandTimeout = timeout.Value;
                    cmd.CommandType = type;
                    if (!sqlParameters.IsNullOrEmpty())
                        cmd.Parameters.AddRange(sqlParameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        public void ExecuteReaderCommand(Action<SqlDataReader> reader, string sql, CommandType type = CommandType.Text, int? timeout = null, bool iterateReader = true,
                             params SqlParameter[] sqlParameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (timeout.HasValue && timeout.Value >= 0)
                        cmd.CommandTimeout = timeout.Value;
                    cmd.CommandType = type;
                    if (!sqlParameters.IsNullOrEmpty())
                        cmd.Parameters.AddRange(sqlParameters);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (iterateReader)
                        {
                            while (r.Read())
                            {
                                reader(r);
                            }
                        }
                        else
                        {
                            reader(r);
                        }
                    }
                }
            }
        }

        public void Execute(Action<SqlConnection> action)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                action(connection);
            }
        }

        public TResult Execute<TResult>(Func<SqlConnection, TResult> function)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                return function(connection);
            }
        }

        public void BeginTransaction(bool readUncommited = true)
        {
            if (_transactionScope == null)
            {
                lock (this)
                {
                    if (_transactionScope == null)
                    {
                        if (readUncommited)
                        {
                            _transactionScope = new TransactionScope(TransactionScopeOption.Required,
                                                                     new TransactionOptions()
                                                                     {
                                                                         IsolationLevel = IsolationLevel.ReadUncommitted
                                                                     });
                        }
                        else
                        {
                            _transactionScope = new TransactionScope();
                        }
                    }
                }
            }
        }

        public void CommitTransaction()
        {
            if (_transactionScope != null)
            {
                lock (this)
                {
                    if (_transactionScope != null)
                    {
                        _transactionScope.Complete();
                    }
                }
            }
        }

        public DateTime GetSystemDate()
        {
            var date = ExecuteScalarCommand("select getDate() as Date") as DateTime?;
            if (date == null)
                throw new Exception("Error retriving system date");
            return date.Value;
        }

        public DateTime GetSystemUtcDate()
        {
            var date = ExecuteScalarCommand("select getUtcDate() as Date") as DateTime?;
            if (date == null)
                throw new Exception("Error retriving system date");
            return date.Value;
        }

        public virtual void Dispose()
        {
            if (_transactionScope != null)
            {
                _transactionScope.Dispose();
            }
        }
    }
}