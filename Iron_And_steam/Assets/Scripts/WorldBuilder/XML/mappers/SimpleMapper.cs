namespace IaS.WorldBuilder.XML.mappers
{
    public class SimpleMapper : IXmlValueMapper<string>
    {
        public string Map(string from)
        {
            return from;
        }
    }
}
