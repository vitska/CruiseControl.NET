using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Cruise.DashboardBroker {
    public static class XmlUtils {
        public static object FromStream(Type t, Stream fs)
        {
            try
            {
                var xmlRd = new XmlTextReader(fs);
                var xr = XmlReader.Create(xmlRd, new XmlReaderSettings { ValidationType = ValidationType.None });
                if (xr == null) throw new Exception(string.Format("XmlReader.Create {0} failed", t.FullName));
                var serializer = new XmlSerializer(t);
                var obj = serializer.Deserialize(xr);
                if (obj == null) throw new Exception(string.Format("{0} deserialization failed", t.FullName));
                return obj;
            }
            finally
            {
                fs.Close();
            }
        }

        public static T FromFile<T>(string filename)
        {
            using(var fs = File.OpenRead(filename)) {
                return (T)FromStream(typeof(T), fs);
            }
        }

        internal static void ToString(object item, XmlWriter xw)
        {
            var serializer = new XmlSerializer(item.GetType());
            serializer.Serialize(xw, item);
        }

        public static string ToString(object item)
        {
            using(var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))//, ConformanceLevel = ConformanceLevel.Fragment
                {
                    ToString(item, xw);
                }
                return sw.ToString();
            }
        }

    }
}
