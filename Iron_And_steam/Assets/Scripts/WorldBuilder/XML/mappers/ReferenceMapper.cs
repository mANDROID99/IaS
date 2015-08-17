using System;
using System.Collections.Generic;
using System.Linq;

namespace IaS.WorldBuilder.XML.mappers
{
    public class ReferenceMapper<T> : IXmlValueMapper<T> where T : IXmlReferenceable
    {
        private readonly IEnumerable<T> _references; 

        public ReferenceMapper(IEnumerable<T> references)
        {
            _references = references;
        }  

        public T Map(string from)
        {
            T reference = _references.FirstOrDefault(r => (ToRefId(r.GetId())).Equals(from));
            if (reference == null)
            {
                throw new Exception(string.Format("Value '{0}', Reference could not be found.", from));
            }
            return reference;
        }

        private static string ToRefId(string id)
        {
            return "@" + id;
        }
    }
}