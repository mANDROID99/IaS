using System;

namespace IaS.WorldBuilder.XML.mappers
{
    public class FloatMapper : IXmlValueMapper<float>
    {
        public float Map(string from)
        {
            float floatValue;
            if (!float.TryParse(from, out floatValue))
            {
                throw new Exception(string.Format("Value '{0}' cannot be converted to a float", from));
            }
            return floatValue;
        }
    }
}