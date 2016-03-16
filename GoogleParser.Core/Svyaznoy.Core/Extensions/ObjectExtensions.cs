using System;
using System.IO;
using System.Xml.Serialization;

namespace Svyaznoy.Core
{
    public static class ObjectExtensions
    {
        public static string ToStringOrNull(this object obj)
        {
            return obj == null ? null : obj.ToString();
        }
        
        public static string ToStringOrEmpty(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        public static T1 Maybe<T0, T1>(this T0 value, Func<T0, T1> f) where T0 : class
        {
            if (value == null)
            {
                return default(T1);
            }

            return f(value);
        }

        public static T1 Maybe<T0, T1>(this T0 value, Func<T0, T1> f, T1 defaultValue) where T0 : class
        {
            if (value == null)
            {
                return defaultValue;
            }

            var result = f(value);
            
            if (result == null)
            {
                return defaultValue;
            }
            else
            {
                return result;
            }
        }


        public static string Serialize<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;
            var serializer = new XmlSerializer(typeof (T));

            string res = null;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                res = writer.ToString();
            }

            return res;
        }

        public static string SerializeWithOutNamespace<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(T));

            string res = null;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj, ns);
                res = writer.ToString();
            }

            return res;
        }
    }
}