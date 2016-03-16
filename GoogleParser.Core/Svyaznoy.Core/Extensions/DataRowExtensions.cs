using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Svyaznoy.Core
{
    public static class DataRowExtensions
    {
        public static string Get(this DataRowView row, string name, string def)
        {
            if (row == null)
                return def;
            return row.Row.Get(name, def);
        }

        public static string Get(this DataRow row, string name, string def)
        {
            try
            {
                var val = row[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return val.ToString();
            }
            catch
            {
                return def;
            }
        }

        

        public static int Get(this DataRowView row, string name, int def)
        {
            if (row == null)
                return def;
            return row.Row.Get(name, def);
        }

        public static Guid Get(this DataRow row, string name, Guid def)
        {
            if (row == null)
                return def;

            Guid res;
            if (Guid.TryParse(row.Get(name, default(Guid).ToString()), out res))
            {
                return res;
            }
            else
            {
                return def;
            }
        }

        public static Guid Get(this DataRowView row, string name, Guid def)
        {
            if (row == null)
                return def;

            Guid res;
            if (Guid.TryParse(row.Row.Get(name, default(Guid).ToString()), out res))
            {
                return res;
            }
            else
            {
                return def;
            }
        }

        public static TEnum Get<TEnum>(this DataRowView row, string name, TEnum def)
            where TEnum : struct
        {
            if (row == null)
                return def;

            return (TEnum)(object)row.Row.Get(name, Convert.ToInt32(def));
        }

        public static int Get(this DataRow row, string name, int def)
        {
            try
            {
                var val = row[name];
                if (val == DBNull.Value)
                    return def;
                else if (val == DBNull.Value)
                    return def;
                else
                {
                    return Convert.ToInt32(val);
                }
            }
            catch
            {
                return def;
            }
        }

        public static bool Get(this DataRowView row, string name, bool def)
        {
            if (row == null)
                return def;
            return row.Row.Get(name, def);
        }

        public static bool Get(this DataRow row, string name, bool def)
        {
            try
            {
                var val = row[name];
                if (val == DBNull.Value)
                    return def;
                else if (val == DBNull.Value)
                    return def;
                else
                    return Convert.ToBoolean(val);
            }
            catch
            {
                return def;
            }
        }

        public static DateTime Get(this DataRowView row, string name, DateTime def)
        {
            if (row == null)
                return def;
            return row.Row.Get(name, def);
        }

        public static DateTime Get(this DataRow row, string name, DateTime def)
        {
            try
            {
                var val = row[name];
                if (val == DBNull.Value)
                    return def;
                else if (val == DBNull.Value)
                    return def;
                else
                    return Convert.ToDateTime(val);
            }
            catch
            {
                return def;
            }
        }

        public static string Get(this SqlDataReader reader, string name, string def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return val.ToString();
            }
            catch
            {
                return def;
            }
        }

        public static bool Get(this SqlDataReader reader, string name, bool def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return (bool)val;
            }
            catch
            {
                return def;
            }
        }

        public static int Get(this SqlDataReader reader, string name, int def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return (int) val;
            }
            catch
            {
                return def;
            }
        }

        public static DateTime Get(this SqlDataReader reader, string name, DateTime def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return (DateTime)val;
            }
            catch
            {
                return def;
            }
        }

        public static double Get(this SqlDataReader reader, string name, double def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return (double)val;
            }
            catch
            {
                return def;
            }
        }

        public static decimal Get(this SqlDataReader reader, string name, decimal def)
        {
            try
            {
                var val = reader[name];
                if (val == DBNull.Value || val == null)
                    return def;
                else
                    return (decimal)val;
            }
            catch
            {
                return def;
            }
        }
    }
}