namespace Svyaznoy.Core
{
    using System.Data;
    using System;
    using System.Collections.Generic;

    public static class DataTableExtensions
    {
        public static IEnumerable<T> Cast<T>(this DataTable data) where T : class, new()
        {
            var list = new List<T>();

            foreach (DataRow row in data.Rows)
            {
                var item = new T();
                var classType = typeof(T);

                foreach (DataColumn column in data.Columns)
                {
                    var property = classType.GetProperty(column.ColumnName);

                    if(property == null)
                        continue;

                    var numberColumn = data.Columns.IndexOf(column);
                    var value = row.ItemArray[numberColumn];

                    if (value != null && value != DBNull.Value)
                    {
                        if (property.PropertyType == typeof(string) && column.DataType != typeof(string))
                            value = value.ToString();
                    }
                    else
                    {
                        if (value == DBNull.Value)
                            value = null;
                    }

                    property.SetValue(item, value, null);
                }

                list.Add(item);
            }

            return list;
        }
    }
}
