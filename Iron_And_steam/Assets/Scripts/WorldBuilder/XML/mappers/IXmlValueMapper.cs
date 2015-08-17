
namespace IaS.WorldBuilder.XML.mappers
{
    interface IXmlValueMapper<out T>
    {
        T Map(string from);
    }
}
