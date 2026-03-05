using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MPFX
{
    public static class Serializer
    {
        public static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new System.Xml.XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", ""); // removes xsi and xsd namespaces

            using var stringWriter = new Utf8StringWriter();
            using var writer = System.Xml.XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(writer, obj, namespaces);
            return stringWriter.ToString();
        }

        // Ensures UTF-8 instead of UTF-16
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));

            using var stringReader = new StringReader(xml);
            return (T)serializer.Deserialize(stringReader);
        }
    }
}
