using UnityEngine;

namespace IaS.Domain.XML.mappers
{
    public class Vector3Mapper : IXmlValueMapper<Vector3>
    {
        public Vector3 Map(string from)
        {
            string[] splitString = from.Split(',');
            return new Vector3(float.Parse(splitString[0].Trim()), float.Parse(splitString[1].Trim()), float.Parse(splitString[2].Trim()));
        }
    }
}