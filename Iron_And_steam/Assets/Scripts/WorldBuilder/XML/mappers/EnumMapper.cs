using System;

namespace IaS.WorldBuilder.XML.mappers
{
    public class EnumMapper<TE> : IXmlValueMapper<TE> where TE : struct
    {
        public TE Map(string from)
        {
            try
            {
                return (TE) Enum.Parse(typeof (TE), from);
            }
            catch (ArgumentException)
            {
                throw new Exception(string.Format("Value '{0}' could not be converted to an Enum", from));
            }
        }
    }
}