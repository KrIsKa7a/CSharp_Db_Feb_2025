namespace CarDealer.Utilities
{
    using System.Text;
    using System.Xml.Serialization;

    public class XmlHelper
    {
        public static T? Deserialize<T>(string inputXml, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(T), xmlRoot);

            using StringReader strReader = new StringReader(inputXml);

            object? deserializedObject = xmlSerializer
                .Deserialize(strReader);
            if (deserializedObject == null)
            {
                return null;
            }

            return (T)deserializedObject;
        }

        public static T? Deserialize<T>(Stream inputXmlStream, string rootName)
            where T : class
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(T), xmlRoot);

            object? deserializedObject = xmlSerializer
                .Deserialize(inputXmlStream);
            if (deserializedObject == null)
            {
                return null;
            }

            return (T)deserializedObject;
        }

        public static string Serialize<T>(T obj, string rootName, Dictionary<string, string>? namespaces = null)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer 
                = new XmlSerializer(typeof(T), xmlRoot);
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            if (namespaces == null)
            {
                xmlNamespaces.Add(string.Empty, string.Empty);
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in namespaces)
                {
                    xmlNamespaces.Add(kvp.Key, kvp.Value);
                }
            }

            using StringWriter stringWriter = new StringWriter(result);

            xmlSerializer.Serialize(stringWriter, obj, xmlNamespaces);

            return result.ToString().TrimEnd();
        }

        public static void Serialize<T>(T obj, string rootName, Stream outputStream, Dictionary<string, string>? namespaces = null)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer
                = new XmlSerializer(typeof(T), xmlRoot);
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            if (namespaces == null)
            {
                xmlNamespaces.Add(string.Empty, string.Empty);
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in namespaces)
                {
                    xmlNamespaces.Add(kvp.Key, kvp.Value);
                }
            }

            xmlSerializer.Serialize(outputStream, obj, xmlNamespaces);
        }
    }
}
