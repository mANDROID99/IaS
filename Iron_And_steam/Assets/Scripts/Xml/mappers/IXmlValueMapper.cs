
namespace IaS.Domain.XML.mappers
{
    interface IXmlValueMapper<out T>
    {
        T Map(string from);
    }
}
