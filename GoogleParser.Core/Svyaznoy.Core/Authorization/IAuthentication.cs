using Svyaznoy.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Authorization
{
    public interface IAuthentication
    {
        IUserIdentity User { get; }
    }

    public interface IAuthentication<out TInventory> : IAuthentication
        where TInventory : IInventory
    {
        TInventory CreateInventory(bool unlimitedTimeout = false, int? timeoutSeconds = null);

        void Invoke(Action<TInventory> action, bool unlimitedTimeout = false, int? timeoutSeconds = null);

        T Invoke<T>(Func<TInventory, T> func, bool unlimitedTimeout = false, int? timeoutSeconds = null);
    }

    public abstract class Authentication<TInventory> : IAuthentication<TInventory>
         where TInventory : IInventory
    {
        protected string ConnectionString { get; private set; }

        protected Authentication(IUserIdentity user, string connectionString)
        {
            User = user;
            ConnectionString = connectionString;
        }

        public abstract TInventory CreateInventory(bool unlimitedTimeout = false, int? timeoutSeconds = null);

        public void Invoke(Action<TInventory> action, bool unlimitedTimeout = false, int? timeoutSeconds = null)
        {
            using (var inventory = CreateInventory(unlimitedTimeout: unlimitedTimeout, timeoutSeconds: timeoutSeconds))
            {
                action(inventory);
            }
        }

        public T Invoke<T>(Func<TInventory, T> func, bool unlimitedTimeout = false, int? timeoutSeconds = null)
        {
            using (var inventory = CreateInventory(unlimitedTimeout: unlimitedTimeout, timeoutSeconds: timeoutSeconds))
            {
                return func(inventory);
            }
        }

        public IUserIdentity User { get; protected set; }

        protected string GetConnectionString(bool unlimitedTimeout = false, int? timeoutSeconds = null)
        {
            if (!unlimitedTimeout && timeoutSeconds == null)
                return ConnectionString;

            var b = new SqlConnectionStringBuilder(ConnectionString);
            if (timeoutSeconds == null)
            {
                b.ConnectTimeout = 0;
            }
            else if (timeoutSeconds > 0)
            {
                b.ConnectTimeout = timeoutSeconds.Value;
            }

            return b.ToString();
        }

    }
}