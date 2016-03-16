using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Svyaznoy.Core
{
    public static class XmlSerializerHelper
    {
        public static XElement SerializeToElement<T>(this T obj)
            where T : class
        {
            if (obj == null)
            {
                return null;
            }
            return XElement.Parse(Serialize(obj));
        }

        public static string Serialize<T>(T obj)
            where T : class
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(default(T)))
            {
                return null;
            }
            using (var sw = new StringWriter())
            {
                var ser = new XmlSerializer(obj.GetType());
                ser.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        public static T Deserialize<T>(this XElement element) where T : class
        {
            if (element == null)
            {
                return null;
            }

            using (var sr = element.CreateReader())
            {
                var ser = new XmlSerializer(typeof(T));
                return (T)ser.Deserialize(sr);
            }
        }

        public static T Deserialize<T>(this string xmlString) where T : class
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }
            if (string.IsNullOrEmpty(xmlString))
            {
                return default(T);
            }

            using (var sr = new StringReader(xmlString))
            {
                var ser = new XmlSerializer(typeof(T));
                return (T)ser.Deserialize(sr);
            }
        }
    }
}