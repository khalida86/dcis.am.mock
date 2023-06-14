using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Dcis.Am.Mock.Icp.Helpers
{
    public class XmlHelper
    {
        public static T DeserializeFromXml<T>(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static string GetXml(object instance)
        {
            XmlNamespaceManager manager;
            var element = XmlHelper.SerializeObjectToXElement(out manager, instance);

            return SerializeToXml(element, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the XML for the specified request
        /// </summary>
        /// <param name="instance">The instance of the object to serialize to XML.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string SanitizeMessageDateAndGetXml(object instance)
        {
            XmlNamespaceManager manager;

            var element = XmlHelper.SerializeObjectToXElement(out manager, instance);

            manager.AddNamespace("ato", "http://www.ato.gov.au/Subjects/EAI/2005/09/ATOCanonical");

            var messageDateTime = element.XPathSelectElement("//ato:Control/ato:MessageDatetime", manager);

            if (messageDateTime != null)
            {
                var value = messageDateTime.Value;

                if (!string.IsNullOrEmpty(value) && value.Contains("."))
                {
                    messageDateTime.Value = value.Substring(0, 19) + value.Substring(value.Length - 6);
                }
            }

            return SerializeToXml(element, Encoding.UTF8);
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to process.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string SerializeToXml(XElement element, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Indent = false,
                        NewLineOnAttributes = false,
                        NewLineHandling = NewLineHandling.Replace,
                        NewLineChars = ""
                    };

                    using (var xmlWriter = XmlWriter.Create(writer, settings))
                    {
                        var document = new XDocument(new XDeclaration("1.0", encoding.WebName, null), element);
                        document.WriteTo(xmlWriter);
                        xmlWriter.Flush();
                        return Encoding.ASCII.GetString(stream.ToArray());
                    }
                }
            }
        }


        /// <summary>
        /// Serializes the specified instance.
        /// </summary>
        /// <param name="namespaceManager">The <see cref="XmlNamespaceManager"/> to return.</param>
        /// <param name="instance">The instance to process.</param>
        /// <returns>Returns <see cref="XElement"/></returns>
        public static XElement SerializeObjectToXElement(out XmlNamespaceManager namespaceManager, object instance)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(instance.GetType());
                serializer.Serialize(stream, instance);
                stream.Position = 0;

                using (var reader = XmlReader.Create(stream))
                {
                    namespaceManager = new XmlNamespaceManager(reader.NameTable);
                    return XElement.Load(reader);
                }
            }
        }

        /// <summary>
        /// Creates an XML Document which ignores namespaces.
        /// It is easier to query the document using XPath when namespaces are ignored.
        /// </summary>
        /// <remarks>
        /// Ignoring namespaces is a bit dodgy but OK for our usage in this test harness.
        /// </remarks>
        public static XmlDocument LoadWithoutNamespace(string xml)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(xml)))
            {
                using (var xmlReader = new XmlTextReader(stream))
                {
                    xmlReader.Namespaces = false;

                    XmlDocument document = new XmlDocument();
                    document.Load(xmlReader);

                    return document;
                }
            }
        }
    }
}
