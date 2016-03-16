using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Svyaznoy.BIP.Utils;
using Svyaznoy.Core.Localization;

namespace Svyaznoy.Core
{
    public static partial class TranslationExtensions
    {
        public static List<TEnum> EnumToList<TEnum>(List<TEnum> except = null)
            where TEnum : struct
        {
            except = except ?? new List<TEnum>();
            var items = new List<TEnum>();
            typeof(TEnum)
                    .GetEnumValues()
                    .AsQueryable()
                    .Cast<TEnum>()
                    .Where(e => !except.Contains(e))
                    .OrderBy(e => e.ToString())
                    .ForEach(items.Add);
            return items;
        }

        public static List<T?> EnumToNullableList<T>()
            where T : struct
        {
            var items = new List<T?> { null };
            EnumToList<T>().ForEach(i => items.Add(i));
            return items;
        }

        public static List<TextEnum<TEnum>> EnumToTranslatedList<TEnum>(this ITranslator translator, bool nullable = false, IEnumerable<TEnum> except = null)
            where TEnum : struct
        {
            var items = new List<TextEnum<TEnum>>();

            typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToList()
                .ForEach(p =>
                {
                    var val = p.GetValue(null);

                    if (!(val is TEnum))
                        return;

                    var enumVal = (TEnum)val;
                    if (except != null && except.Contains(enumVal))
                        return;

                    var trAttr =
                        (TranslationAttribute)
                        p.GetCustomAttributes(typeof(TranslationAttribute), true).FirstOrDefault();

                    var text = trAttr != null && translator != null ? translator.TranslateValue(trAttr.Key) : enumVal.ToString();

                    items.Add(new TextEnum<TEnum>(enumVal, text));
                });

            items = items.OrderBy(t => t.Text).ToList();
            if (nullable)
            {
                items.Insert(0, new TextEnum<TEnum>());
            }
            return items;
        }

        public static string EnumValueToTranslatedText<TEnum>(this ITranslator translator, TEnum value)
        {
            string text = null;
            foreach (var p in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var val = p.GetValue(null);

                if (!(val is TEnum))
                    continue;

                var enumVal = (TEnum)val;
                if (Equals(enumVal, value))
                {
                    var trAttr =
                        (TranslationAttribute)
                        p.GetCustomAttributes(typeof(TranslationAttribute), true).FirstOrDefault();

                    text = trAttr != null && translator != null
                               ? translator.TranslateValue(trAttr.Key)
                               : enumVal.ToString();
                    break;
                }
            }
            return text;
        }

        public static string Translit(this string value)
        {
            if (value == null)
                return null;

            var sb = new StringBuilder();

            foreach (var c in value)
            {
                string ch;
                switch (c)
                {
                    case 'а':
                        ch = "a";
                        break;
                    case 'б':
                        ch = "b";
                        break;
                    case 'в':
                        ch = "v";
                        break;
                    case 'г':
                        ch = "g";
                        break;
                    case 'д':
                        ch = "d";
                        break;
                    case 'е':
                        ch = "e";
                        break;
                    case 'ё':
                        ch = "jo";
                        break;
                    case 'ж':
                        ch = "zh";
                        break;
                    case 'з':
                        ch = "z";
                        break;
                    case 'и':
                        ch = "i";
                        break;
                    case 'й':
                        ch = "j";
                        break;
                    case 'к':
                        ch = "k";
                        break;
                    case 'л':
                        ch = "l";
                        break;
                    case 'м':
                        ch = "m";
                        break;
                    case 'н':
                        ch = "n";
                        break;
                    case 'о':
                        ch = "o";
                        break;
                    case 'п':
                        ch = "p";
                        break;
                    case 'р':
                        ch = "r";
                        break;
                    case 'с':
                        ch = "s";
                        break;
                    case 'т':
                        ch = "t";
                        break;
                    case 'у':
                        ch = "u";
                        break;
                    case 'ф':
                        ch = "f";
                        break;
                    case 'х':
                        ch = "h";
                        break;
                    case 'ц':
                        ch = "c";
                        break;
                    case 'ч':
                        ch = "ch";
                        break;
                    case 'ш':
                        ch = "sh";
                        break;
                    case 'щ':
                        ch = "sch";
                        break;
                    case 'ъ':
                        ch = "";
                        break;
                    case 'ы':
                        ch = "y";
                        break;
                    case 'ь':
                        ch = "'";
                        break;
                    case 'э':
                        ch = "je";
                        break;
                    case 'ю':
                        ch = "yu";
                        break;
                    case 'я':
                        ch = "ja";
                        break;
                    case 'А':
                        ch = "A";
                        break;
                    case 'Б':
                        ch = "B";
                        break;
                    case 'В':
                        ch = "V";
                        break;
                    case 'Г':
                        ch = "G";
                        break;
                    case 'Д':
                        ch = "D";
                        break;
                    case 'Е':
                        ch = "E";
                        break;
                    case 'Ё':
                        ch = "Jo";
                        break;
                    case 'Ж':
                        ch = "Zh";
                        break;
                    case 'З':
                        ch = "Z";
                        break;
                    case 'И':
                        ch = "I";
                        break;
                    case 'Й':
                        ch = "J";
                        break;
                    case 'К':
                        ch = "K";
                        break;
                    case 'Л':
                        ch = "L";
                        break;
                    case 'М':
                        ch = "M";
                        break;
                    case 'Н':
                        ch = "N";
                        break;
                    case 'О':
                        ch = "O";
                        break;
                    case 'П':
                        ch = "P";
                        break;
                    case 'Р':
                        ch = "R";
                        break;
                    case 'С':
                        ch = "S";
                        break;
                    case 'Т':
                        ch = "T";
                        break;
                    case 'У':
                        ch = "U";
                        break;
                    case 'Ф':
                        ch = "F";
                        break;
                    case 'Х':
                        ch = "H";
                        break;
                    case 'Ц':
                        ch = "C";
                        break;
                    case 'Ч':
                        ch = "Ch";
                        break;
                    case 'Ш':
                        ch = "Sh";
                        break;
                    case 'Щ':
                        ch = "Sch";
                        break;
                    case 'Ъ':
                        ch = "";
                        break;
                    case 'Ы':
                        ch = "Y";
                        break;
                    case 'Ь':
                        ch = "'";
                        break;
                    case 'Э':
                        ch = "Je";
                        break;
                    case 'Ю':
                        ch = "Yu";
                        break;
                    case 'Я':
                        ch = "Ja";
                        break;
                    default:
                        ch = c.ToString();
                        break;
                }
                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string WithPostfixByNumber(this string itemName, int number)
        {
            const string p1 = "ов"; // например 45 товаров
            const string p2 = ""; // например 31 товар
            const string p3 = "а"; // например // 2 товара

            var sti = new[] { p1, p2, p3 };
            var index = number % 100;
            if (index >= 11 && index <= 14) index = 0;
            else index = (index %= 10) < 5 ? (index > 2 ? 2 : index) : 0;

            return itemName + sti[index];
        }
    }
}