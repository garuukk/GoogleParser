using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Svyaznoy.Core
{
    public static class StringExtensions
    {
        public static string SafeTrim(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        public static string GetFirst(this string str, int lenght)
        {
            if (str == null)
            {
                return null;
            }
            return str.Length >= lenght ? str.Substring(0, lenght) : str;
        }

        public static string FormatBy(this string str, params object[] @params)
        {
            if (string.IsNullOrWhiteSpace((str)))
                return str;
            return string.Format(str, @params);
        }

        public static bool ConvertToBool(this string str, bool def)
        {
            bool val;
            int intVal;
            if (bool.TryParse(str, out val))
            {
                return val;
            }
            else if(int.TryParse(str, out intVal))
            {
                return intVal != 0;
            }
            else
            {
                return def;
            }
        }

        public static int ConvertToInt(this string str, int def)
        {
            if (string.IsNullOrWhiteSpace(str))
                return def;
            int res = int.MinValue;
            if (int.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return def;
            }
        }

        public static Int64 ConvertToLong(this string str, int def)
        {
            if (string.IsNullOrWhiteSpace(str))
                return def;
            Int64 res = Int64.MinValue;
            if (Int64.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return def;
            }
        }



        public static int? ConvertToInt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            int res = int.MinValue;
            if (int.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public static decimal? ConvertToDecimal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            
            var res = decimal.MinValue;
            if (decimal.TryParse(str,NumberStyles.Any, NumberFormatInfo.InvariantInfo, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public static decimal ConvertToDecimal(this string str, decimal def)
        {
            return str.ConvertToDecimal() ?? def;
        }

        public static Guid? ConvertToGuid(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            Guid res = Guid.Empty;
            if (Guid.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public static Guid ConvertToGuid(this string str, Guid @default)
        {
            return ConvertToGuid(str) ?? @default;
        }

        public static double? ConvertToDouble(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            double res = double.MinValue;
            if (double.TryParse(str, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public static double ConvertToDouble(this string str, double def)
        {
            return ConvertToDouble(str) ?? def;
        }

        /// <summary>
        /// Конвертирует строку в DateTime, использует текущий формат даты-времени
        /// </summary>
        /// <param name="str"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string str, DateTime def)
        {
            return str.ConvertToDateTime() ?? def;
        }

        /// <summary>
        /// Конвертирует строку в DateTime, использует текущий формат даты-времени
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? ConvertToDateTime(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            DateTime dt;
            if (DateTime.TryParse(str, out dt))
                return dt;
            return null;
        }

        public static string ToSqlString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.Replace("'", "''");
        }

        public static HtmlString ToHtmlParagraphs(this string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return new HtmlString("");

            return
                new HtmlString(String.Format("<p>{0}</p>",
                    HttpContext.Current.Server.HtmlEncode(str.Trim()).Replace(Environment.NewLine, "</p><p>")));
        }

        public static string UpperCaseFirst(this string str)
        {
            if (string.IsNullOrEmpty(str)) 
                return str;

            if (str.Length == 1) return str.ToUpperInvariant();
            
            string newValue = str.ToLowerInvariant();
            newValue = newValue[0].ToString().ToUpperInvariant() + newValue.Substring(1, newValue.Length - 1);
            return newValue;
        }
    }

    public class StringIgnorCaseComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
                return 0;

            return obj.GetHashCode();
        }
    }
}