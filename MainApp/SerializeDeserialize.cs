using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CustomSerilization
{
    public class SerializeDeserialize<T>
    { 
        StringBuilder sbData;
        StringWriter swWriter;
        XmlDocument xDoc;
        XmlNodeReader xNodeReader;
        XmlSerializer xmlSerializer;

        public SerializeDeserialize()
        {
            sbData = new StringBuilder();
        }

        public string SerializeData(T data)
        {
            //XmlRootAttribute xRoot = new XmlRootAttribute
            //{
            //    ElementName = "ISSUE",
            //    Namespace = "http://www.w3.org/2001/XMLSchema-instance",
            //    IsNullable = true
            //};

            //xmlSerializer = new XmlSerializer( typeof(T), xRoot);
            xmlSerializer = new XmlSerializer(typeof(T));
            swWriter = new StringWriter(sbData);

            xmlSerializer.Serialize(swWriter, data);
            return sbData.ToString();
        }

        public T DeserializeData(string dataXML)
        {
            xDoc = new XmlDocument();
            xDoc.LoadXml(dataXML);

            xNodeReader = new XmlNodeReader(xDoc.DocumentElement);

            xmlSerializer = new XmlSerializer(typeof(T));

            var deserializedData = xmlSerializer.Deserialize(xNodeReader);
            T deserializedT = (T)deserializedData;

            return deserializedT;
        }

    }
}