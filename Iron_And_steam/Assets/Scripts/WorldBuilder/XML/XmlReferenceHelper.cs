using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder.XML;

namespace IaS.XML
{
    public class XmlReferenceHelper
    {

        public static T GetReference<T>(string refId, IEnumerable<T> objects) where T : IXmlReferenceable
        {
            return objects.FirstOrDefault(o => (ToRefId(o.GetId())).Equals(refId));
        }

        private static string ToRefId(string id)
        {
            return "@" + id;
        }
    }
}
