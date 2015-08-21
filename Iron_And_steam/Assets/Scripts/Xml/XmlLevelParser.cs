using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace IaS.Xml
{
    public class XmlLevelParser
    {

        public XmlLevel ParseLevel(TextAsset levelAsset)
        {
            XmlReader sourceReader = XmlReader.Create(new StringReader(levelAsset.text));
            XDocument xDoc = new XDocument(XDocument.Load(sourceReader));
            XElement xLevel = xDoc.Element(XmlLevel.ElementLevel);
            return XmlLevel.FromElement(xLevel);
        }

    }
}
